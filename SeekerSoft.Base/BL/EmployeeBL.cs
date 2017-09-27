using SeekerSoft.Base.DB;
using SeekerSoft.Base.DTO;
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

namespace SeekerSoft.Base.BL
{
    public class EmployeeBL
    {
        public static string EmpHeaderPath = ConfigHelper.Get("EmpHeaderPath");   //员工头像图片
        public static string TempPath = ConfigHelper.Get("TempPath");   //临时存放路径

        public static PagerResult<MEmployee> EmployeeQuery(UserInfo userInfo, QEmployee pagerParams)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT  t1.EmpID ,
        t1.DeptID ,
        t1.UserID ,
        t1.Name ,
        t1.EmpCode ,
        t1.Gender ,
        t1.Tel ,
        t1.EMail ,
        t1.Status ,
		DepartmentName=t2.Name,
        t1.Remark ,
		t3.LoginName,
		GenderName=t4.Name
	    FROM  base.Employee t1
		LEFT JOIN base.Department t2 ON t2.DeptID = t1.DeptID
		LEFT JOIN base.LoginUser t3 ON t3.UserID = t1.UserID
		LEFT JOIN base.DicItem t4 ON t1.Gender=t4.Code AND t4.DicCode='1015'
		LEFT JOIN base.DicItem t5 ON t1.Status=t5.Code AND t5.DicCode='1003'
		WHERE t1.IsDel=0");
            if (!String.IsNullOrEmpty(pagerParams.KeyWord))
            {
                sb.AppendFormat("AND( t1.EmpCode LIKE '%{0}%' OR t1.Name LIKE '%{0}%')", pagerParams.KeyWord.Replace("'", "''"));
            }
            if (!String.IsNullOrWhiteSpace(pagerParams.Status))
            {
                sb.AppendFormat("AND t1.Status ='{0}'", pagerParams.Status);
            }
            if (!String.IsNullOrWhiteSpace(pagerParams.DeptID)&& pagerParams.DeptID!="1")
            {
                sb.AppendFormat("AND t2.DeptID ='{0}'", pagerParams.DeptID);
            }
            if (!String.IsNullOrWhiteSpace(pagerParams.FullDeptID))
            {
                sb.AppendFormat("AND t2.FullDeptID LIKE'{0}%'", pagerParams.FullDeptID);
            }
            //排序
            if (string.IsNullOrEmpty(pagerParams.sortField)) pagerParams.sortOrder = "t1.Name";
            switch (pagerParams.sortField)
            {
                case "Name":
                    pagerParams.sortField = "t1.Name";
                    break;
            }

            sb.AppendFormat(" ORDER BY {0}", SqlPageProcedureParams.GetSortSQL(pagerParams.sortField, pagerParams.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = pagerParams.pageSize;
            pageProcedureParams.pageIndex = pagerParams.pageIndex;
            pageProcedureParams.sql = sb.ToString();
            using (var db = DBHelper.NewDB())
            {
                PagerResult<MEmployee> data = DBHelper<MEmployee>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        public static MEmployee GetByID(int id)
        {
            string sql = @"SELECT  t1.EmpID ,
        t1.DeptID ,
        t1.UserID ,
        t1.Name ,
        t1.EmpCode ,
        t1.Gender ,
        t1.NFCID ,
        t1.Tel ,
        t1.EMail ,
        t1.Status ,
        t1.Remark ,
		DepartmentName=t2.Name,
		t3.LoginName,
        t1.PDCode
		 FROM base.Employee t1
		 LEFT JOIN base.Department t2 ON t2.DeptID = t1.DeptID
		 LEFT JOIN base.LoginUser t3 ON t3.UserID = t1.UserID
		 WHERE t1.EmpID= " + id;
            using (var db = DB.DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var data = DBHelper<MEmployee>.ExecuteSqlToEntities(db, sql).SingleOrDefault();
                if (data == null)
                    throw new Exception("找不到指定的数据。");
                return data;
            }
        }

        public static MEmployee GetByNFCID(string nfcid)
        {
            string sql = @"SELECT  t1.EmpID ,
        t1.DeptID ,
        t1.UserID ,
        t1.Name ,
        t1.EmpCode ,
        t1.Gender ,
        t1.NFCID ,
        t1.Tel ,
        t1.EMail ,
        t1.Status ,
        t1.Remark ,
		DepartmentName=t2.Name,
		t3.LoginName
		 FROM base.Employee t1
		 LEFT JOIN base.Department t2 ON t2.DeptID = t1.DeptID
		 LEFT JOIN base.LoginUser t3 ON t3.UserID = t1.UserID
		 WHERE t1.IsDel=0 AND RIGHT(t1.NFCID,8)= RIGHT(@nfcid,8)";
            using (var db = DB.DBHelper.NewDB())
            {
                return DBHelper<MEmployee>.ExecuteSqlToEntities(db, sql, new SqlParameter("nfcid", nfcid)).FirstOrDefault();
            }
        }

        public static bool EmployeeAdd(UserInfo userinfo, MEmployee model)
        {
            using (var db = DBHelper.NewDB())
            {

                Employee empEntity = new Employee()
                {
                    Name = model.Name,
                    EmpCode = model.EmpCode,
                    Gender = model.Gender,
                    DeptID = model.DeptID,
                    NFCID = model.NFCID,
                    Tel = model.Tel,
                    EMail = model.EMail,
                    Status = "0",
                    Remark = model.Remark,
                    IsDel = false,
                    CreateBy = userinfo.UserID,
                    CreateTime = DateTime.Now,
                    PDCode=model.PDCode
                };
                //新增账号
                if (!string.IsNullOrWhiteSpace(model.LoginName))
                {
                    model.LoginName = model.LoginName.Trim();
                    if (db.LoginUser.Any(_ => !_.IsDel && _.LoginName == model.LoginName))
                        throw new Exception("该账号名已被使用！");

                    var userEntity = db.LoginUser.Add(new LoginUser()
                    {
                        LoginName = model.LoginName,
                        Pwd = !string.IsNullOrEmpty(ConfigHelper.Get("defaultPassword")) ? ConfigHelper.Get("defaultPassword") : "123456",
                    });
                    db.SaveChanges();
                    empEntity.UserID = userEntity.UserID;
                }

                db.Employee.Add(empEntity);
                db.SaveChanges();

                string tempPhoto = HttpContext.Current.Request.MapPath(TempPath) + model.PhotoFile;
                string newPhoto = HttpContext.Current.Request.MapPath(EmpHeaderPath) + model.EmpCode + ".jpg";

                // 保存照片
                if (!string.IsNullOrEmpty(model.PhotoFile)
                    && File.Exists(tempPhoto))
                {
                    if (File.Exists(newPhoto)) File.Delete(newPhoto);
                    File.Move(tempPhoto, newPhoto);
                }
            }
            return true;
        }

        public static bool EmployeeSave(UserInfo userinfo, MEmployee model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Employee.Where(_ => _.EmpID == model.EmpID).SingleOrDefault();
                if (entity == null) throw new Exception("找不到指定的数据");
                if (!string.IsNullOrWhiteSpace(model.EmpCode)
                    && db.Employee.Any(_ => _.EmpCode == model.EmpCode && _.EmpID != model.EmpID && !_.IsDel && _.Status == "0"))
                    throw new Exception("工号已被占用");

                if (!string.IsNullOrWhiteSpace(model.NFCID)
                    && db.Employee.Any(_ => _.NFCID == model.NFCID && _.EmpID != model.EmpID && !_.IsDel && _.Status == "0"))
                    throw new Exception("NFCID已使用");

                string oldPhoto = HttpContext.Current.Request.MapPath(EmpHeaderPath) + entity.EmpCode + ".jpg";
                string newPhoto = HttpContext.Current.Request.MapPath(EmpHeaderPath) + model.EmpCode + ".jpg";
                string tempPhoto = HttpContext.Current.Request.MapPath(TempPath) + model.PhotoFile;

                // 移动头像
                if (entity.EmpCode != model.EmpCode)
                {
                    // 工号为空，删除原头像
                    if (string.IsNullOrEmpty(model.EmpCode))
                    {
                        if (File.Exists(oldPhoto))
                            File.Delete(oldPhoto);
                    }
                    else
                    {// 工号有且上传了头像，则更新头像
                        if (File.Exists(tempPhoto) && File.Exists(newPhoto))
                            File.Delete(newPhoto);
                        if (File.Exists(oldPhoto))
                            File.Move(oldPhoto, newPhoto);
                    }

                    entity.EmpCode = model.EmpCode;
                }

                entity.Name = model.Name;
                entity.Gender = model.Gender;
                entity.DeptID = model.DeptID;
                entity.NFCID = model.NFCID;
                entity.Tel = model.Tel;
                entity.EMail = model.EMail;
                entity.Remark = model.Remark;
                entity.UpdateTime = DateTime.Now;
                entity.ModifyBy = userinfo.UserID;
                entity.PDCode = model.PDCode;
                if (!entity.UserID.HasValue && !string.IsNullOrWhiteSpace(model.LoginName))
                {
                    if (db.LoginUser.Any(_ => !_.IsDel && _.LoginName == model.LoginName))
                        throw new Exception("该账号名已被使用！");

                    var userEntity = db.LoginUser.Add(new LoginUser()
                    {
                        LoginName = model.LoginName,
                        Pwd = !string.IsNullOrEmpty(ConfigHelper.Get("defaultPassword")) ? ConfigHelper.Get("defaultPassword") : "123456",
                    });
                    db.SaveChanges();
                    entity.UserID = userEntity.UserID;
                }
                db.SaveChanges();

                // 保存照片
                if (!string.IsNullOrEmpty(model.PhotoFile)
                    && File.Exists(tempPhoto))
                {
                    if (File.Exists(newPhoto)) File.Delete(newPhoto);
                    File.Move(tempPhoto, newPhoto);
                }
            }
            return true;
        }

        //设置离职在职
        public static bool SetDisabledEmployee(UserInfo userifo, int id, bool isDisabled)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.Employee.Where(_ => _.EmpID == id).SingleOrDefault();
                if (m == null) throw new Exception("数据不存在。");

                m.Status = isDisabled ? "0" : "1";  // 0在职  1离职
                if (m.Status == "1")
                {
                    m.NFCID = ""; //离职的时候NFCID置空
                }

                if (m.UserID != null)
                {
                    var dbo = db.LoginUser.Where(_ => _.UserID == m.UserID).SingleOrDefault();
                    dbo.IsDisabled = !isDisabled;
                }

                db.SaveChanges();
            }
            return true;
        }



        public static MDepartment[] GetDepartmentName(UserInfo userInfo)
        {
            string sql = @"SELECT  DeptID ,
        Name ,
        PID 
		FROM base.Department";
            using (var db = DBHelper.NewDB())
            {
                return DBHelper<MDepartment>.ExecuteSqlToEntities(db, sql).ToArray();
            }
        }

        //获取维修人员列表
        public static MEmployee[] GetRepairPerson(UserInfo userInfo)
        {
            string sql = @"SELECT  t1.UserID,t1.Name
FROM    base.Employee t1
        INNER JOIN base.UserRole t2 ON t2.UserID = t1.UserID
        INNER JOIN base.RoleFuncMap t3 ON t3.RoleID = t2.RoleID
WHERE   t3.FuncCode = '1902'";
            using (var db = DB.DBHelper.NewDB())
            {
                var data = DBHelper<MEmployee>.ExecuteSqlToEntities(db, sql).ToArray();
                return data;
            }
        }
    }
}
