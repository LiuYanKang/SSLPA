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
    public class EmployeeBL
    {
        //获取LPA员工信息
        public static MEmployee EmployeeGet(int id)
        {
            string sql = @"SELECT  t1.EmpID ,
		EmpName=t2.Name,
        t1.Position ,
        t1.SuperiorID ,
		SuperiorName=t3.Name,
        t1.IsResponsible,t1.AuditType,t1.Section  FROM lpa.Employee t1
		LEFT JOIN base.Employee t2 ON t2.EmpID = t1.EmpID
		LEFT JOIN base.Employee t3 ON t1.SuperiorID=t3.EmpID
		WHERE t1.IsDel=0 AND t1.EmpID=@id ";
            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MEmployee>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");

                model.AuditArea = db.EmpAuditArea.Where(_ => _.EmpID == id).Select(_ => _.AuditArea).ToArray();

                return model;
            }
        }

        //新增LPA员工信息
        public static bool EmployeeSave(UserInfo userinfo, MEmployee model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Employee.Where(_ => _.EmpID == model.EmpID).SingleOrDefault();
                //如果数据库中存在这条数据，则进行编辑操作
                if (entity != null)
                {
                    entity.UpdateTime = DateTime.Now;
                    entity.ModifyBy = userinfo.UserID;
                    entity.AuditType = model.AuditType;
                    entity.Section = model.Section;
                }
                else
                {
                    //新增
                    entity = new Employee()
                    {
                        EmpID = model.EmpID,
                        CreateBy = userinfo.UserID,
                        CreateTime = DateTime.Now,
                        AuditType=model.AuditType,
                        Section=model.Section
                    };
                    db.Employee.Add(entity);
                }
                entity.Position = "";
                entity.SuperiorID = model.SuperiorID;
                entity.IsResponsible = false;
                entity.IsDel = false;
                db.SaveChanges();
                db.EmpAuditArea.RemoveRange(db.EmpAuditArea.Where(_ => _.EmpID == model.EmpID));
                // 保存授权区域
                if (model.AuditArea != null && model.AuditArea.Length > 0)
                {
                    for (int i = 0; i < model.AuditArea.Length; i++)
                    {
                        if (string.IsNullOrEmpty(model.AuditArea[i]))
                            continue;
                        db.EmpAuditArea.Add(new EmpAuditArea()
                        {
                            EmpID = entity.EmpID,
                            AuditArea = model.AuditArea[i]
                        });
                    }
                    db.SaveChanges();
                }
            }
            return true;
        }

        //查询LPA员工信息
        public static PagerResult<MEmployee> EmployeeList(UserInfo userInfo, QEmployee param)
        {
            StringBuilder sqlWhere = new StringBuilder();
            sqlWhere.Append(@"
 lpa.Employee t1
	LEFT JOIN base.Employee t2 ON t2.EmpID = t1.EmpID
	LEFT JOIN base.DicItem t3 ON t1.AuditType=t3.Code AND t3.DicCode='1024'
WHERE t1.IsDel=0 ");

            // 非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sqlWhere.AppendFormat(" AND t2.PDCode = '{0}' ", pdCode);
                }
            }
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sqlWhere.AppendFormat("  AND (t2.Name LIKE '%{0}%'  )", param.Keyword.Replace("'", "''"));
            }
            if (param.Status != null)
            {
                sqlWhere.AppendFormat(" AND t1.IsResponsible = '{0}' ", param.Status);
            }

            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = "ItemID";
            

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.searchField = @"
t1.EmpID ,
	EmpName=t2.Name,
    t1.AuditType ,
	AuditTypeName= t3.Name,
    t1.IsResponsible  ,
	AuditAreaSummary =  STUFF((SELECT ','+ are.AreaName FROM lpa.EmpAuditArea eaa INNER JOIN lpa.Area are ON are.AreaId=eaa.AuditArea  WHERE eaa.EmpID=t1.EmpID FOR XML PATH('')),1,1,'') 
";
            pageProcedureParams.afterFromBeforeOrderBySql = sqlWhere.ToString();

            switch (param.sortField) {
                case "EmpID":
                    param.sortField = "t1.EmpID";
                    break;
                case "EmpName":
                    param.sortField = "t2.Name";
                    break;
                //case "SuperiorName":
                //    param.sortField = "t3.Name";
                //    break;
            }

            pageProcedureParams.orderByField = SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder);

            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MEmployee>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        //删除LPA员工信息
        public static bool EmployeeDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.Employee.Where(_ => _.EmpID == id).FirstOrDefault();
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        public static MArea[] GetAuditArea(int planId)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                string sql = @"SELECT are.AreaId,are.AreaName FROM lpa.PlanAreaMap pam INNER JOIN lpa.Area are ON pam.AreaId=are.AreaId WHERE pam.PlanID=@planId";

                return DBHelper<MArea>.ExecuteSqlToEntities(db, sql, new SqlParameter("planId", planId)).ToArray();
            }
        }
    }
}
