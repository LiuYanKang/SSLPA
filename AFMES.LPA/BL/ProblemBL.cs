using SSLPA.DB;
using SSLPA.LPA.DTO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SeekerSoft.Core;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.Config;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SeekerSoft.Base.DTO;
using MEmployee = SeekerSoft.Base.DTO.MEmployee;

namespace SSLPA.LPA.BL
{
    public class ProblemBL
    {

        public static string LPAImgPath = ConfigHelper.Get("LPAImgPath");   //LPA问题图片
        public static string TempPath = ConfigHelper.Get("TempPath");   //临时存放路径



        //获取问题管理
        public static MProblem ProblemGet(int id)
        {
            string sql = @"SELECT  t1.ProbID ,
        t1.ActionID ,
        t1.ItemID ,
        t1.ProblemRegion ,
		ProblemRegionName=t2.AreaName,
        t1.ProblemType ,
		ProblemTypeName=t3.Name,
        t1.Responsible ,
		ResponsibleName=t4.Name,
        t1.MachineID ,
		MachineName=t7.Name,
        t1.ProblemDesc ,
        t1.SubmitDate ,
        t1.PlanStartDate ,
        t1.PlanEndDate ,
        t1.ActualEndDate ,
        t1.Measure ,
        t1.State ,
		StateName=t5.Name,
        t1.Progress ,
        t1.Remark,
		CreateByName=t6.Name
	    FROM lpa.Problem t1
		LEFT JOIN lpa.Area t2 ON t1.ProblemRegion=t2.AreaId 
		LEFT JOIN base.DicItem t3 ON t1.ProblemType=t3.Code AND t3.DicCode='1026'
		LEFT JOIN base.Employee t4 ON t1.Responsible=t4.EmpID
		LEFT JOIN base.DicItem t5 ON t1.State=t5.Code AND t5.DicCode='1029'
		LEFT JOIN base.Employee t6 ON t1.CreateBy=t6.EmpID
		LEFT JOIN dic.Machine t7 ON t1.MachineID = t7.MachineID
		WHERE t1.IsDel=0 AND t1.ProbID=@id ";

            string sql2 = @"SELECT  t1.PicID ,
		        t1.ProbID ,
		        t1.PicType ,
		        t1.FileName ,
		        t1.IsDel ,
		        t1.CreateTime ,
		        t1.CreateBy FROM lpa.ProblemPic t1
				LEFT JOIN lpa.Problem t2 ON t2.ProbID = t1.ProbID
				WHERE t1.PicType=1 AND t1.ProbID=@id2";

            string sql3 = @"SELECT  t1.PicID ,
		        t1.ProbID ,
		        t1.PicType ,
		        t1.FileName ,
		        t1.IsDel ,
		        t1.CreateTime ,
		        t1.CreateBy FROM lpa.ProblemPic t1
				LEFT JOIN lpa.Problem t2 ON t2.ProbID = t1.ProbID
				WHERE t1.PicType=2 AND t1.ProbID=@id3";

            string sql4 = @"SELECT  t1.LogID ,
        t1.ProbID ,
        t1.PlanEndDate ,
        t1.NewPlanEndDate 
	    FROM lpa.ProblemLog t1
		LEFT JOIN lpa.Problem t2 ON t2.ProbID = t1.ProbID
		WHERE t1.ProbID=@id4";

            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                model.BeforeProbPicList = DBHelper<MProblemPic>.ExecuteSqlToEntities(db, sql2, new SqlParameter("@id2", id)).ToArray();
                model.AfterProbPicList = DBHelper<MProblemPic>.ExecuteSqlToEntities(db, sql3, new SqlParameter("@id3", id)).ToArray();
                model.ProbLogList = DBHelper<MProblemLog>.ExecuteSqlToEntities(db, sql4, new SqlParameter("@id4", id)).ToArray();

                return model;
            }
        }

        //保存问题管理数据
        public static bool ProblemSave(UserInfo userinfo, MProblem model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Problem.Where(_ => _.ProbID == model.ProbID).SingleOrDefault();
                if (entity == null) throw new Exception("找不到指定的数据");

                var entityPlanEndDate = entity.PlanEndDate;

                entity.ProblemRegion = model.ProblemRegion.Value ;
                entity.ProblemType = model.ProblemType;
                entity.MachineID = model.MachineID;
                entity.Responsible = model.Responsible.Value ;
                entity.ProblemDesc = model.ProblemDesc;
                entity.SubmitDate = model.SubmitDate;
                entity.PlanStartDate = model.PlanStartDate;
                entity.PlanEndDate = model.PlanEndDate;
                entity.ActualEndDate = model.ActualEndDate;
                entity.Measure = model.Measure;
                entity.Progress = model.Progress.Value ;
                if (model.Progress == 100)
                {
                    entity.State = "2";
                }
                entity.Remark = model.Remark;
                entity.ModifyBy = userinfo.UserID;
                entity.UpdateTime = DateTime.Now;

                //如果计划完成日期有更改的话，则更新修改日志表
                if (model.PlanEndDate != entityPlanEndDate)
                {
                    db.ProblemLog.Add(new ProblemLog()
                    {
                        ProbID = entity.ProbID,
                        PlanEndDate = model.OldPlanEndDate,
                        NewPlanEndDate = model.PlanEndDate,
                        CreateTime = DateTime.Now,
                        CreateBy = userinfo.UserID
                    });
                }

                //获取发现问题前 图片表里的数据
                var oldItems = db.ProblemPic.Where(_ => _.ProbID == model.ProbID && _.PicType == "1").ToList();
                //删除：遍历原纪录，但在新数据中不存在的，差异化处理
                var delItems = oldItems.Where(_ => !model.BeforeProbPicList.Any(newitem => newitem.PicID == _.PicID)).ToList();
                foreach (var pic in delItems)
                {
                    db.ProblemPic.Remove(pic);
                    if (File.Exists(HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName))
                    {
                        File.Delete(HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName);
                    }
                }
                foreach (var pic in model.BeforeProbPicList)
                {
                    if (pic.PicID == null)
                    {
                        File.Move(HttpContext.Current.Request.MapPath(TempPath) + pic.FileName, HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName);
                        db.ProblemPic.Add(new ProblemPic()
                        {
                            ProbID = entity.ProbID,
                            PicType = "1",
                            FileName = pic.FileName,
                            CreateTime = DateTime.Now,
                            CreateBy = userinfo.UserID
                        });
                    }
                }


                //获取处理问题后 图片表里的数据
                var oldItems2 = db.ProblemPic.Where(_ => _.ProbID == model.ProbID && _.PicType == "2").ToList();
                //删除：遍历原纪录，但在新数据中不存在的，差异化处理
                var delItems2 = oldItems2.Where(_ => !model.AfterProbPicList.Any(newitem => newitem.PicID == _.PicID)).ToList();
                foreach (var pic in delItems2)
                {
                    db.ProblemPic.Remove(pic);
                    if (File.Exists(HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName))
                    {
                        File.Delete(HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName);
                    }
                }
                foreach (var pic in model.AfterProbPicList)
                {
                    if (pic.PicID == null)
                    {
                        File.Move(HttpContext.Current.Request.MapPath(TempPath) + pic.FileName, HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName);
                        db.ProblemPic.Add(new ProblemPic()
                        {
                            ProbID = entity.ProbID,
                            PicType = "2",
                            FileName = pic.FileName,
                            CreateTime = DateTime.Now,
                            CreateBy = userinfo.UserID
                        });
                    }
                }


                db.SaveChanges();
            }
            return true;
        }

        //查询问题管理
        public static PagerResult<MProblem> ProblemList(UserInfo userInfo, QProblem param)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT  t1.ProbID ,
        t1.ActionID ,
        t1.ItemID ,
		ItemDesc = t7.Description ,
        t1.ProblemRegion ,
		ProblemRegionName=t2.AreaName,
        t1.ProblemType ,
		ProblemTypeName=t3.Name,
        t1.MachineID ,
		MachineName=t8.Name,
        t1.Responsible ,
		ResponsibleName=t4.Name,
        t1.ProblemDesc ,
        t1.SubmitDate ,
        t1.PlanStartDate ,
        t1.PlanEndDate ,
        t1.ActualEndDate ,
        t1.Measure ,
        t1.State ,
		StateName=t5.Name,
        t1.Progress ,
        t1.Remark,
		CreateByName=t6.Name,UserId={0}
FROM lpa.Problem t1
		LEFT JOIN lpa.Area t2 ON t1.ProblemRegion=t2.AreaId 
		LEFT JOIN base.DicItem t3 ON t1.ProblemType=t3.Code AND t3.DicCode='1026'
		LEFT JOIN base.Employee t4 ON t1.Responsible=t4.EmpID
		LEFT JOIN base.DicItem t5 ON t1.State=t5.Code AND t5.DicCode='1029'
		LEFT JOIN base.Employee t6 ON t1.CreateBy=t6.EmpID
		LEFT JOIN lpa.AuditItem t7 ON t1.ItemID=t7.ItemID
		LEFT JOIN dic.Machine t8 ON t8.MachineID = t1.MachineID
		LEFT JOIN base.Department t9 ON t4.DeptID = t9.DeptID
WHERE t1.IsDel=0 ", userInfo.EmpID);
            // 权限条件
            // 非LPA管理员 只能查看发现人是自己的或责任人是自己的
            if (!userInfo.AuthList.Contains("2003"))
            {
                sql.AppendFormat(@"AND (t1.Responsible = {0} OR t1.CreateBy = {0}
 OR t9.ManagerId={0} )", userInfo.EmpID ?? 0); // 比较特殊，LPA的问题创建人记录的也是EmpID

                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t4.PDCode = '{0}' ", pdCode);
                }
            }
            // 其他筛选条件
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat(" AND ( t4.Name LIKE '%{0}%' OR  t1.ProblemDesc LIKE '%{0}%' OR  t1.Measure LIKE '%{0}%' OR  t1.Remark LIKE '%{0}%'  OR  t2.AreaName LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
            }
            if (param.State != null)
            {
                sql.AppendFormat(" AND t1.State = '{0}' ", param.State);
            }
            if (param.MachineID != null)
            {
                sql.AppendFormat(" AND t1.MachineID = '{0}' ", param.MachineID);
            }
            if (param.ProblemRegionState != null)
            {
                sql.AppendFormat(" AND t1.ProblemRegion = '{0}' ", param.ProblemRegionState);
            }
            if (param.ProblemTypeState != null)
            {
                sql.AppendFormat(" AND t1.ProblemType = '{0}' ", param.ProblemTypeState);
            }
            if (param.SubmitBeginDate != null)
            {
                sql.AppendFormat(@" AND t1.SubmitDate >= '{0}' ", param.SubmitBeginDate.Value.Date);
            }
            if (param.SubmitEndDate != null)
            {
                sql.AppendFormat(@" AND t1.SubmitDate < '{0}' ", param.SubmitEndDate.Value.AddDays(1));
            }
            if (param.Progress.HasValue)
            {
                if (param.Progress.Value == -1)
                    sql.Append(@" AND t1.Progress < 100 ");
                else
                    sql.AppendFormat(@" AND t1.Progress = '{0}' ", param.Progress.Value);
            }
            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = " t1.SubmitDate ";
            switch (param.sortField)
            {
                case "ItemDesc":
                    param.sortField = "t7.Description";
                    break;
                case "ProblemRegionName":
                    param.sortField = "t2.AreaName";
                    break;
                case "ProblemTypeName":
                    param.sortField = "t3.Name";
                    break;
                case "ResponsibleName":
                    param.sortField = "t4.Name";
                    break;
                case "StateName":
                    param.sortField = "t5.Name";
                    break;
                case "CreateByName":
                    param.sortField = "t6.Name";
                    break;
                case "MachineName":
                    param.sortField = "t8.Name";
                    break;
            }

            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MProblem>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        public static bool ProblemUpdate(UserInfo userInfo, MProblem model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Problem.Where(_ => _.ProbID == model.ProbID).SingleOrDefault();
                if (entity == null) throw new Exception("找不到指定的数据");

                var entityPlanEndDate = entity.PlanEndDate;

                entity.PlanStartDate = model.PlanStartDate;
                entity.PlanEndDate = model.PlanEndDate;
                entity.Measure = model.Measure;
                entity.MachineID = model.MachineID;
                entity.Progress = model.Progress.Value ;
                if (model.Progress == 100)
                {
                    entity.State = "2";
                }
                entity.ModifyBy = userInfo.UserID;
                entity.UpdateTime = DateTime.Now;

                //如果计划完成日期有更改的话，则更新修改日志表
                if (model.PlanEndDate != entityPlanEndDate)
                {
                    db.ProblemLog.Add(new ProblemLog()
                    {
                        ProbID = entity.ProbID,
                        PlanEndDate = model.OldPlanEndDate,
                        NewPlanEndDate = model.PlanEndDate,
                        CreateTime = DateTime.Now,
                        CreateBy = userInfo.UserID
                    });
                }

                //获取处理问题后 图片表里的数据
                var oldItems2 = db.ProblemPic.Where(_ => _.ProbID == model.ProbID && _.PicType == "2").ToList();
                //删除：遍历原纪录，但在新数据中不存在的，差异化处理
                var delItems2 = oldItems2.Where(_ => !model.AfterProbPicList.Any(newitem => newitem.PicID == _.PicID)).ToList();
                foreach (var pic in delItems2)
                {
                    db.ProblemPic.Remove(pic);
                    if (File.Exists(HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName))
                    {
                        File.Delete(HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName);
                    }
                }
                foreach (var pic in model.AfterProbPicList)
                {
                    if (pic.PicID == null)
                    {
                        File.Move(HttpContext.Current.Request.MapPath(TempPath) + pic.FileName, HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName);
                        db.ProblemPic.Add(new ProblemPic()
                        {
                            ProbID = entity.ProbID,
                            PicType = "2",
                            FileName = pic.FileName,
                            CreateTime = DateTime.Now,
                            CreateBy = userInfo.UserID
                        });
                    }
                }

                db.SaveChanges();
            }
            return true;
        }


        public static bool ProblemFinish(UserInfo userInfo, MProblem model)
        {
            using (var db = DBHelper.NewDB())
            {
                int managerEmpID = 0;
                var entity = db.Problem.Where(_ => _.ProbID == model.ProbID).SingleOrDefault();
                if (entity == null) throw new Exception("找不到指定的数据");

                string sql = @"SELECT ISNULL(d.ManagerId, 0) as ManagerId
                                FROM base.Department d JOIN base.Employee e ON d.DeptID = e.DeptID
                                WHERE e.UserID = @userid ";
                var data = DBHelper<MDepartment>.ExecuteSqlToEntities(db, sql, new SqlParameter("userid", entity.Responsible)).FirstOrDefault();
                if (data != null&& data.ManagerId != null)
                {
                    managerEmpID= data.ManagerId.Value;
                }
                if (managerEmpID != userInfo.EmpID.Value && userInfo.UserID != 1)// 管理员除外
                    throw new Exception("关闭问题请联系部门经理");
                entity.Remark = model.Remark;
                entity.Progress = 100;
                entity.State = "2";
                entity.ModifyBy = userInfo.UserID;
                entity.UpdateTime = DateTime.Now;

                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// App 个人问题列表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static MProblem[] MineProblemList(UserInfo userInfo)
        {
            string sql = @"
SELECT  t1.ProbID ,
        t1.ProblemRegion ,
        t1.SubmitDate ,
        ProblemRegionName = t2.AreaName ,
        t1.ProblemType ,
        ProblemTypeName = t3.Name ,
        t1.MachineID ,
		MachineName=t6.Name,
        t1.Responsible ,
        t1.ProblemDesc ,
        t1.PlanEndDate ,
        t1.ActualEndDate ,
        t1.Measure ,
        t1.Progress ,
        t1.CreateBy ,
        CreateByName = t5.Name
FROM    lpa.Problem t1
        LEFT JOIN lpa.Area t2 ON t1.ProblemRegion = t2.AreaId 
        LEFT JOIN base.DicItem t3 ON t1.ProblemType = t3.Code AND t3.DicCode = '1026'
        LEFT JOIN base.Employee t5 ON t1.CreateBy = t5.EmpID
		LEFT JOIN dic.Machine t6 ON t6.MachineID = t1.MachineID
WHERE   t1.IsDel = 0
        --AND t1.State IN ( '1', '2' )
        AND t1.Progress < 100
		AND t1.Responsible = @empid
ORDER BY t1.SubmitDate DESC ";

            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql
                    , new SqlParameter("empid", userInfo.EmpID)).ToArray();
                return data;
            }
        }


        //获取问题详情
        public static MProblem ProblemInfo(UserInfo userInfo, int id)
        {
            string sql = @"SELECT  t1.ProbID ,
        t1.ActionID ,
        t1.ItemID ,
        t1.ProblemRegion ,
		ProblemRegionName=t2.AreaName,
        t1.ProblemType ,
		ProblemTypeName=t3.Name,
        t1.MachineID ,
		MachineName=t7.Name,
        t1.Responsible ,
		ResponsibleName=t4.Name,
        t1.ProblemDesc ,
        t1.SubmitDate ,
        t1.PlanStartDate ,
        t1.PlanEndDate ,
        t1.ActualEndDate ,
        t1.Measure ,
        t1.State ,
		StateName=t5.Name,
        t1.Progress ,
        t1.Remark,
		CreateByName=t6.Name,t1.ImproveAdvice  
	    FROM lpa.Problem t1
		LEFT JOIN lpa.Area t2 ON t1.ProblemRegion=t2.AreaId 
		LEFT JOIN base.DicItem t3 ON t1.ProblemType=t3.Code AND t3.DicCode='1026'
		LEFT JOIN base.Employee t4 ON t1.Responsible=t4.EmpID
		LEFT JOIN base.DicItem t5 ON t1.State=t5.Code AND t5.DicCode='1029'
		LEFT JOIN base.Employee t6 ON t1.CreateBy=t6.EmpID
		LEFT JOIN dic.Machine t7 ON t7.MachineID = t1.MachineID
		WHERE t1.IsDel=0 AND t1.ProbID=@id ";

            string sql2 = @"SELECT  t1.PicID ,
		        t1.ProbID ,
		        t1.PicType ,
		        t1.FileName ,
		        t1.IsDel ,
		        t1.CreateTime ,
		        t1.CreateBy FROM lpa.ProblemPic t1
				LEFT JOIN lpa.Problem t2 ON t2.ProbID = t1.ProbID
				WHERE t1.PicType=1 AND t1.ProbID=@id2";

            string sql3 = @"SELECT  t1.PicID ,
		        t1.ProbID ,
		        t1.PicType ,
		        t1.FileName ,
		        t1.IsDel ,
		        t1.CreateTime ,
		        t1.CreateBy FROM lpa.ProblemPic t1
				LEFT JOIN lpa.Problem t2 ON t2.ProbID = t1.ProbID
				WHERE t1.PicType=2 AND t1.ProbID=@id3";

            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                model.BeforeProbPicList = DBHelper<MProblemPic>.ExecuteSqlToEntities(db, sql2, new SqlParameter("@id2", id)).ToArray();
                model.AfterProbPicList = DBHelper<MProblemPic>.ExecuteSqlToEntities(db, sql3, new SqlParameter("@id3", id)).ToArray();

                return model;
            }
        }


        //问题看板数据
        public static MProblem[] KanBanProblemList(int? machineid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"
SELECT  t1.ProbID ,
        t1.ProblemRegion ,
        t1.SubmitDate ,
        ProblemRegionName = t2.AreaName ,
        t1.ProblemType ,
        ProblemTypeName = t3.Name ,
        t1.Responsible ,
        ResponsibleName = t4.Name ,
        t1.ProblemDesc ,
        t1.PlanEndDate ,
        t1.ActualEndDate ,
        t1.Measure ,
        t1.Progress ,
        t1.CreateBy ,
        CreateByName = t5.Name
FROM    lpa.Problem t1
        LEFT JOIN lpa.Area t2 ON t1.ProblemRegion = t2.AreaId 
        LEFT JOIN base.DicItem t3 ON t1.ProblemType = t3.Code AND t3.DicCode = '1026'
        LEFT JOIN base.Employee t4 ON t1.Responsible = t4.EmpID
        LEFT JOIN base.Employee t5 ON t1.CreateBy = t5.EmpID
WHERE   t1.IsDel = 0
        AND t1.State IN ( '1', '2' )
        AND ( t1.Progress < 100 OR DATEADD(DAY, 7, t1.ActualEndDate) > GETDATE() )
");
            if(machineid.HasValue && machineid.Value>0)
                sql.AppendFormat("AND t1.MachineID = {0} ", machineid);
            sql.Append("ORDER BY t1.SubmitDate DESC");

            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql.ToString()).ToArray();
                return data;
            }
        }

        /// <summary>
        /// add by lidm 2017-05-22：查看问题详情
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
//        public static MProblem KanBanProblemDetail(int pid)
//        {
//            string sql = @"SELECT  t1.ProbID ,
//        t1.ProblemRegion ,
//        t1.SubmitDate ,
//        ProblemRegionName = t2.Name ,
//        t1.ProblemType ,
//        ProblemTypeName = t3.Name ,
//        t1.Responsible ,
//        ResponsibleName = t4.Name ,
//        t1.PlanStartDate,
//        t1.ProblemDesc ,
//        t1.PlanEndDate ,
//        t1.ActualEndDate ,
//        t1.Measure ,
//        t1.Progress ,
//        t1.CreateBy ,
//        CreateByName = t5.Name
//FROM    lpa.Problem t1
//        LEFT JOIN base.DicItem t2 ON t1.ProblemRegion = t2.Code AND t2.DicCode = '1025'
//        LEFT JOIN base.DicItem t3 ON t1.ProblemType = t3.Code AND t3.DicCode = '1026'
//        LEFT JOIN base.Employee t4 ON t1.Responsible = t4.EmpID
//        LEFT JOIN base.Employee t5 ON t1.CreateBy = t5.EmpID
//WHERE   t1.IsDel = 0 AND t1.ProbID=@id
//        AND t1.State IN ( '1', '2' )
//        AND ( t1.Progress < 100 OR DATEADD(DAY, 7, t1.ActualEndDate) > GETDATE() ) ";

//            string sql2 = @"SELECT  t1.PicID ,
//		        t1.ProbID ,
//		        t1.PicType ,
//		        t1.FileName ,
//		        t1.IsDel ,
//		        t1.CreateTime ,
//		        t1.CreateBy FROM lpa.ProblemPic t1
//				LEFT JOIN lpa.Problem t2 ON t2.ProbID = t1.ProbID
//				WHERE t1.PicType=1 AND t1.ProbID=@id2";

//            string sql3 = @"SELECT  t1.PicID ,
//		        t1.ProbID ,
//		        t1.PicType ,
//		        t1.FileName ,
//		        t1.IsDel ,
//		        t1.CreateTime ,
//		        t1.CreateBy FROM lpa.ProblemPic t1
//				LEFT JOIN lpa.Problem t2 ON t2.ProbID = t1.ProbID
//				WHERE t1.PicType=2 AND t1.ProbID=@id3";

//            string sql4 = @"SELECT  t1.LogID ,
//        t1.ProbID ,
//        t1.PlanEndDate ,
//        t1.NewPlanEndDate 
//	    FROM lpa.ProblemLog t1
//		LEFT JOIN lpa.Problem t2 ON t2.ProbID = t1.ProbID
//		WHERE t1.ProbID=@id4";

//            using (var db = DBHelper.NewDB())
//            {
//                var data = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql,new SqlParameter("@id",pid)).SingleOrDefault();
//                data.BeforeProbPicList = DBHelper<MProblemPic>.ExecuteSqlToEntities(db, sql2, new SqlParameter("@id2", pid)).ToArray();
//                data.AfterProbPicList = DBHelper<MProblemPic>.ExecuteSqlToEntities(db, sql3, new SqlParameter("@id3", pid)).ToArray();
//                data.ProbLogList = DBHelper<MProblemLog>.ExecuteSqlToEntities(db, sql4, new SqlParameter("@id4", pid)).ToArray();
//                return data;
//            }

//        }

        //删除问题管理
        public static bool ProblemDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.Problem.Where(_ => _.ProbID == id).FirstOrDefault();
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = (int)userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }


        //导出
        public static string ProblemExport(UserInfo userinfo, QProblem param)
        {
            param.pageIndex = 0;
            param.pageSize = 10 * 10000;// 为了性能考虑，限制10W条
            var data = ProblemList(userinfo, param);

            // 创建工作簿
            IWorkbook hssfworkbook = new XSSFWorkbook();
            // 创建表
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            // 创建标题行
            IRow row = sheet1.CreateRow(0);
            row.CreateCell(0).SetCellValue("发现人");
            row.CreateCell(1).SetCellValue("区域");
            row.CreateCell(2).SetCellValue("分类");
            row.CreateCell(3).SetCellValue("检查项");
            row.CreateCell(4).SetCellValue("描述");
            row.CreateCell(5).SetCellValue("改正措施");
            row.CreateCell(6).SetCellValue("责任人");
            row.CreateCell(7).SetCellValue("提交日期");
            row.CreateCell(8).SetCellValue("计划开始日期");
            row.CreateCell(9).SetCellValue("计划完成日期");
            row.CreateCell(10).SetCellValue("实际完成日期");
            row.CreateCell(11).SetCellValue("完成进度");
            // 插入数据行
            for (int i = 0; i < data.Data.Count; i++)
            {
                row = sheet1.CreateRow(i + 1);
                row.CreateCell(0).SetCellValue(data.Data[i].CreateByName);
                row.CreateCell(1).SetCellValue(data.Data[i].ProblemRegionName);
                row.CreateCell(2).SetCellValue(data.Data[i].ProblemTypeName);
                row.CreateCell(3).SetCellValue(data.Data[i].ItemDesc);
                row.CreateCell(4).SetCellValue(data.Data[i].ProblemDesc);
                row.CreateCell(5).SetCellValue(data.Data[i].Measure);
                row.CreateCell(6).SetCellValue(data.Data[i].ResponsibleName);
                row.CreateCell(7).SetCellValue(data.Data[i].SubmitDate.Value.ToString("yyyy-MM-dd"));
                row.CreateCell(8).SetCellValue(data.Data[i].PlanStartDate.HasValue ? data.Data[i].PlanStartDate.Value.ToString("yyyy-MM-dd") : "");
                row.CreateCell(9).SetCellValue(data.Data[i].PlanEndDate.HasValue ? data.Data[i].PlanEndDate.Value.ToString("yyyy-MM-dd") : "");
                row.CreateCell(10).SetCellValue(data.Data[i].ActualEndDate.HasValue ? data.Data[i].ActualEndDate.Value.ToString("yyyy-MM-dd") : "");
                row.CreateCell(11).SetCellValue(data.Data[i].Progress + "%");
            }
            // 导出文件夹
            string dirName = HttpContext.Current.Request.MapPath("~/Temp/");
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            // 随机生成文件名
            string filename = Guid.NewGuid().ToString() + ".xlsx";
            string fullFilename = Path.Combine(dirName, filename);
            // 输出Excel文件
            using (FileStream fs = new FileStream(fullFilename, FileMode.CreateNew))
            {
                try
                {
                    hssfworkbook.Write(fs);
                }
                catch (Exception ex)
                {
                    Log.Error(typeof(ProblemBL).ToString(), ex.ToString());
                    throw ex;
                }
            }
            return filename;
        }

    }
}
