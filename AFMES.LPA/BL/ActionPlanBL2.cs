using SSLPA.DB;
using SSLPA.LPA.DTO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.BL
{
    public class ActionPlanBL2
    {
        //获取PLA执行计划
        public static MActionPlan2 ActionPlanGet(int id)
        {
            string sql = @"SELECT  t1.PlanID ,
        t1.EmpID ,
		EmpName=t2.Name,
        t1.StartPlanDate ,
        t1.EndPlanDate ,
        t1.AuditType ,
		AuditTypeName=t3.Name,
        t1.IsComplete ,
        t1.ActionTime,
        t1.BanCi 
	    FROM lpa.ActionPlan t1
		LEFT JOIN base.Employee t2 ON t2.EmpID = t1.EmpID
		LEFT JOIN base.DicItem t3 ON t1.AuditType=t3.Code AND t3.DicCode='1024'
		WHERE t1.IsDel=0 AND t1.PlanID=@id ";
            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MActionPlan2>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");
                return model;
            }
        }

        //新增PLA执行计划
        public static bool ActionPlanAdd(UserInfo userinfo, MActionPlan2 model)
        {
            using (var db = DBHelper.NewDB())
            {
                var exists = db.ActionPlan.FirstOrDefault(_ => !_.IsDel && _.EmpID == model.EmpID &&
                    ((_.StartPlanDate >= model.StartPlanDate && _.StartPlanDate <= model.EndPlanDate)
                        || (_.EndPlanDate >= model.StartPlanDate && _.EndPlanDate <= model.EndPlanDate)));
                if (exists != null)       //一个员工的当天只有一条计划
                {
                    throw new Exception("该员工在 " + exists.StartPlanDate.ToShortDateString() + " 已存在计划！");
                }
                exists = new ActionPlan();
                if (model.AuditType == "1")         //日审核
                {
                    for (int i = 0; i < model.dayCount; i++)
                    {
                        var days = model.StartPlanDate.AddDays(i).Date;
                        exists.EmpID = model.EmpID;
                        exists.StartPlanDate = days;
                        exists.EndPlanDate = days;
                        exists.AuditType = model.AuditType;
                        exists.IsDel = false;
                        exists.CreateBy = userinfo.UserID;
                        exists.CreateTime = DateTime.Now;
                        exists.BanCi = model.BanCi;
                        //新增
                        db.ActionPlan.Add(exists);
                    }
                }
                else
                {
                    //非日审核
                    exists.EmpID = model.EmpID;
                    exists.StartPlanDate = model.StartPlanDate;
                    exists.EndPlanDate = model.EndPlanDate;
                    exists.AuditType = model.AuditType;
                    exists.IsDel = false;
                    exists.CreateBy = userinfo.UserID;
                    exists.CreateTime = DateTime.Now;
                    exists.BanCi = model.BanCi;
                    db.ActionPlan.Add(exists);
                }
                db.SaveChanges();

                // 保存审核区域
                if (model.AuditArea != null && model.AuditArea.Length > 0)
                {
                    for (int i = 0; i < model.AuditArea.Length; i++)
                    {
                        if (model.AuditArea[i] <= 0)
                            continue;
                        db.PlanAreaMap.Add(new PlanAreaMap()
                        {
                            PlanID = exists.PlanID,
                            AreaId = model.AuditArea[i]
                        });
                    }
                    db.SaveChanges();
                }

            }
            return true;
        }

        ////保存PLA执行计划数据
        //public static bool ActionPlanSave(UserInfo userinfo, MActionPlan2 model)
        //{
        //    using (var db = DBHelper.NewDB())
        //    {
        //        var entity = db.ActionPlan.Where(_ => _.PlanID == model.PlanID).SingleOrDefault();
        //        if (entity == null) throw new Exception("找不到指定的数据");

        //        entity.EmpID = model.EmpID;
        //        entity.StartPlanDate = model.StartPlanDate;
        //        entity.EndPlanDate = model.EndPlanDate;
        //        entity.AuditType = model.AuditType;
        //        entity.ModifyBy = userinfo.UserID;
        //        entity.UpdateTime = DateTime.Now;
        //        db.SaveChanges();
        //    }
        //    return true;
        //}

        //查询PLA执行计划
        public static PagerResult<MActionPlan2> ActionPlanList(UserInfo userInfo, QActionPlan param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"lpa.ActionPlan t1
		LEFT JOIN base.Employee t2 ON t2.EmpID = t1.EmpID
		LEFT JOIN base.DicItem t3 ON t1.AuditType=t3.Code AND t3.DicCode='1024'
        LEFT JOIN base.DicItem t4 ON t1.BanCi=t4.Code And t4.DicCode='1030'
		WHERE t1.IsDel=0 ");
            // 非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode=SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t2.PDCode = '{0}' ", pdCode);
                }
            }
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat("  AND ( t2.Name LIKE '%{0}%' OR t1.PlanID LIKE '%{0}%' )",
                    param.Keyword.Replace("'", "''"));
            }
            if (param.Status != null)
            {
                sql.AppendFormat(" AND t1.AuditType = '{0}' ", param.Status);
            }
            if (param.PlanBeginDate != null)
            {
                sql.AppendFormat(@" AND t1.StartPlanDate >= '{0}' ", param.PlanBeginDate.Value.Date);
            }
            if (param.PlanEndDate != null)
            {
                sql.AppendFormat(@" AND t1.StartPlanDate < '{0}' ", param.PlanEndDate.Value.Date);
            }
            if (param.ActionBeginTime != null)
            {
                sql.AppendFormat(@" AND t1.ActionTime >= '{0}' ", param.ActionBeginTime.Value.Date);
            }
            if (param.ActionEndTime != null)
            {
                sql.AppendFormat(@" AND t1.ActionTime < '{0}' ", param.ActionEndTime.Value.Date);
            }


            // 排序
            if (string.IsNullOrEmpty(param.sortField)) { param.sortField = " t1.StartPlanDate "; param.sortOrder = "DESC"; }
            switch (param.sortField)
            {
                case "EmpName":
                    param.sortField = "t2.Name";
                    break;
                case "AuditTypeName":
                    param.sortField = "t3.Name";
                    break;
                case "BanCiName":
                    param.sortField = "t1.BanCi";
                    break;
            }

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.searchField = @"t1.PlanID ,
        t1.EmpID ,
		EmpName=t2.Name,
        t1.StartPlanDate ,
        t1.EndPlanDate ,
        t1.AuditType ,
		AuditTypeName=t3.Name,
        t1.IsComplete ,
        t1.ActionTime,
        BanCiName=t4.Name,
        AudtiAreaMes=STUFF((SELECT ','+ are.AreaName FROM lpa.PlanAreaMap pam INNER JOIN lpa.Area are ON pam.AreaId=are.AreaId WHERE pam.PlanID=t1.PlanID  FOR XML PATH('')),1,1,'') ";
            pageProcedureParams.afterFromBeforeOrderBySql = sql.ToString();
            pageProcedureParams.orderByField = SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder);
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MActionPlan2>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        //删除PLA执行计划
        public static bool ActionPlanDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.ActionPlan.Where(_ => _.PlanID == id).FirstOrDefault();
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = (int)userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }


        //批量删除
        public static bool ActionPlanDelForBatch(UserInfo userinfo, int[] rowsID)
        {
            using (var db = DBHelper.NewDB())
            {

                foreach (var delID in rowsID)
                {
                    var m = db.ActionPlan.Where(_ => _.PlanID == delID).FirstOrDefault();

                    if (m == null) throw new Exception("数据不存在");

                    m.IsDel = true;
                    m.ModifyBy = userinfo.UserID;
                    m.UpdateTime = DateTime.Now;
                }
                db.SaveChanges();
            }
            return true;
        }


        /// <summary>
        /// 获取审核区域
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static MArea[] AuditAreaGet(UserInfo userInfo,int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var wheresql = string.Empty;
                if (id > 0)
                {
                    var m = db.Database.SqlQuery<MArea>(@"SELECT PDCode FROM base.Employee WHERE  PDCode!='' AND EmpID=" + id).ToArray();
                    if (m.Length > 0)
                    {
                        wheresql = " AND t1.PDCode='" + m[0].PDCode + "'";
                    }
                }
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(
                        @"SELECT t1.AreaId ,t1.AreaCode ,t1.AreaName ,t1.UpdateTime ,UpdateName = t2.Name,t1.CreateTime
                          FROM   lpa.Area AS t1 WITH ( NOLOCK )
                          LEFT JOIN base.Employee t2 ON t1.ModifyBy = t2.UserID
                          WHERE  1 = 1 AND t1.IsDel = 0 {0}", wheresql);
                // 非LPA管理员 只能查看本部门数据
                if (!userInfo.AuthList.Contains("2003"))
                {
                    var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                    if (!string.IsNullOrEmpty(pdCode))
                    {
                        sql.AppendFormat(" AND t2.PDCode = '{0}' ", pdCode);
                    }
                }
                return db.Database.SqlQuery<MArea>(sql.ToString()).ToArray();
            }
        }

        /// <summary>
        /// 获取员工的审核类型和区域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MSetActionPlan SetActionPlan(int id)
        {
            string sql = @"SELECT t1.AuditType  FROM lpa.Employee t1
		                  WHERE t1.IsDel=0 AND t1.EmpID=@id ";
            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MSetActionPlan>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");

                model.AuditArea = db.EmpAuditArea.Where(_ => _.EmpID == id).Select(_ => _.AuditArea).ToArray();

                return model;
            }
        }

    }
}
