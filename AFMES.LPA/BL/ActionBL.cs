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
    public class ActionBL
    {

        //获取PLA执行计划
        public static MAction2 Get(int id)
        {
            string sql = @"
SELECT  t1.ActionID ,
	t1.PlanID ,
	t1.AuditDate ,
	t1.ProductName ,
	t1.AuditArea ,
	AuditAreaName =t5.AreaName,
	t2.EmpID,
	EmpName=t3.Name,
	t1.State ,
	StateName=t4.Name
FROM lpa.Action t1
	LEFT JOIN lpa.ActionPlan t2 ON t2.PlanID = t1.PlanID
	LEFT JOIN base.Employee t3 ON t2.EmpID=t3.EmpID
	LEFT JOIN base.DicItem t4 ON t1.State=t4.Code AND t4.DicCode='1027'
	LEFT JOIN lpa.Area t5 ON t1.AuditArea=t5.AreaId 
WHERE t1.IsDel=0 AND t1.ActionID=@id ";

            string sql2 = @"SELECT  t1.ActionID ,
						t2.ItemRegion,
						ItemRegionName=t3.AreaName,
				        t1.ItemID ,
                        t2.Description ,
				        t1.Result,t1.EnterNum FROM lpa.LPAActionResult t1
						LEFT JOIN lpa.AuditItem t2 ON t2.ItemID = t1.ItemID
						LEFT JOIN lpa.Area t3 ON t2.ItemRegion=t3.AreaId
						WHERE t1.ActionID=@id2";

            string sql3 = @"SELECT  t1.ProbID ,
        t1.ActionID ,
        t1.ItemID ,
        t1.ProblemRegion ,
		ProblemRegionName=t2.AreaName,
        t1.ProblemType ,
		ProblemTypeName=t3.Name,
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
		CreateByName=t6.Name
	    FROM lpa.Problem t1
		LEFT JOIN lpa.Area t2 ON t1.ProblemRegion=t2.AreaId 
		LEFT JOIN base.DicItem t3 ON t1.ProblemType=t3.Code AND t3.DicCode='1026'
		LEFT JOIN base.Employee t4 ON t1.Responsible=t4.EmpID
		LEFT JOIN base.DicItem t5 ON t1.State=t5.Code AND t5.DicCode='1029'
		LEFT JOIN base.Employee t6 ON t1.CreateBy=t6.EmpID
		LEFT JOIN lpa.Action t7 ON t7.ActionID = t1.ActionID
		WHERE t1.IsDel=0 AND t7.ActionID=@id3";

            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MAction2>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                model.LPAActionResultList = DBHelper<MLPAActionResult>.ExecuteSqlToEntities(db, sql2, new SqlParameter("@id2", id)).ToArray();
                model.ProblemList = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql3, new SqlParameter("@id3", id)).ToArray();

                return model;
            }
        }
        //获取检查项结果
        public static MLPAActionResult[] GetResultItems(int id, string area, string keyword)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT  t1.ActionID ,
						t2.ItemRegion,
						ItemRegionName=t3.AreaName,
				        t1.ItemID ,
                        t2.Description ,
				        t1.Result,t1.EnterNum FROM lpa.LPAActionResult t1
						LEFT JOIN lpa.AuditItem t2 ON t2.ItemID = t1.ItemID
						LEFT JOIN lpa.Area t3 ON t2.ItemRegion=t3.AreaId 
						WHERE t1.ActionID=@id2 ");

            if (!string.IsNullOrEmpty(area))
            {
                sql.AppendFormat(" AND t2.ItemRegion = '{0}' ", area);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql.AppendFormat(" AND t2.Description LIKE '%{0}%' ", keyword.Replace("'", "''"));
            }
            using (var db = DBHelper.NewDB())
            {
                return DBHelper<MLPAActionResult>.ExecuteSqlToEntities(db, sql.ToString(), new SqlParameter("@id2", id)).ToArray();
            }
        }


        //查询PLA执行计划
        public static PagerResult<MAction2> ActionList(UserInfo userInfo, QAction param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"
SELECT  t1.ActionID ,
	t1.PlanID ,
	t1.AuditDate ,
	t1.ProductName ,
	t1.AuditArea ,
	AuditAreaName =t5.AreaName,
	t2.EmpID,
	EmpName=t3.Name,
	t1.State ,
	StateName=t4.Name
FROM lpa.Action t1
	LEFT JOIN lpa.ActionPlan t2 ON t2.PlanID = t1.PlanID
	LEFT JOIN base.Employee t3 ON t2.EmpID=t3.EmpID
	LEFT JOIN base.DicItem t4 ON t1.State=t4.Code AND t4.DicCode='1027'
	LEFT JOIN lpa.Area t5 ON t1.AuditArea=t5.AreaId 
WHERE t1.IsDel=0 ");
            // 非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t3.PDCode = '{0}' ", pdCode);
                }
            }
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat(" AND ( t3.Name LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
            }
            if (param.State != null)
            {
                sql.AppendFormat(" AND t1.State = '{0}' ", param.State);
            }

            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = " t1.AuditDate ";

            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MAction2>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        //删除PLA执行计划
        public static bool ActionDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.Action.Where(_ => _.ActionID == id).FirstOrDefault();
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = (int)userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }




        // //录入数据的审核项汇总
        public static PagerResult<MAuditItem> AuditSummaryList(UserInfo userInfo, QAction param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT  t1.ActionID ,
						t2.ItemRegion,
						ItemRegionName=t3.AreaName,
				        t1.ItemID ,
                        t2.Description ,
				        t1.Result,t1.EnterNum FROM lpa.LPAActionResult t1
						LEFT JOIN lpa.AuditItem t2 ON t2.ItemID = t1.ItemID
						LEFT JOIN lpa.Area t3 ON t2.ItemRegion=t3.AreaId 
						WHERE t2.IsInputData=1");
            //非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t3.PDCode = '{0}' ", pdCode);
                }
            }

            if (param.Area != null)
            {
                sql.AppendFormat(" AND t3.AreaName = '{0}' ", param.Area);
            }
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat("  AND ( t1.ActionID LIKE '%{0}%'OR t2.Description LIKE '%{0}%'  )", param.Keyword.Replace("'", "''"));
            }
            

            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = " t1.ActionID ";

            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MAuditItem>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }


    }
}
