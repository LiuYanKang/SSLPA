using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using SeekerSoft.Base.DB;
using SeekerSoft.Base.DTO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;

namespace SeekerSoft.Base.BL
{
    public class DepartmentBL
    {
        public static MDepartment[] MDepartmentAll()
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var q = from dic in db.Department.AsNoTracking()
                        where dic.IsDel==false
                        select new MDepartment()
                        {
                            DeptID = dic.DeptID,
                            Name = dic.Name,
                            PID = dic.PID,
                            FullDeptID=dic.FullDeptID,
                            ManagerId = dic.ManagerId,
                            Remark = dic.Remark
                        };

                return q.ToArray();
            }
        }


        public static bool DepartmentAdd(UserInfo userinfo, MDepartment model)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                Department dep = new Department();
                dep.Name = model.Name;
                dep.PID = model.PID;
                dep.Remark = model.Remark;
                dep.CreateBy = userinfo.UserID;
                dep.CreateTime = DateTime.Now;
                dep.ModifyBy = dep.CreateBy;
                dep.UpdateTime = dep.CreateTime;
                dep.ManagerId = model.ManagerId;
                dep.IsDel = false;
                dep.SN = model.SN;
                db.Department.Add(dep);
                db.SaveChanges();
                var parentDepartment = db.Department.FirstOrDefault(m => m.DeptID == dep.PID);
                if (parentDepartment != null)
                {
                    dep.FullDeptID = parentDepartment.FullDeptID + "." + dep.DeptID;
                }
               return db.SaveChanges()>0;
            }
        }

        public static bool DepartmentSave(UserInfo userinfo, MDepartment model)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                Department dep = db.Department.FirstOrDefault(m => m.DeptID == model.DeptID);
                if (dep != null)
                {
                    dep.Name = model.Name;
                    dep.PID = model.PID;
                    dep.Remark = model.Remark;
                    dep.ModifyBy = userinfo.UserID;
                    dep.UpdateTime =DateTime.Now;
                    dep.ManagerId = model.ManagerId;
                    dep.IsDel = false;
                    dep.SN = model.SN;
                    var parentDepartment = db.Department.FirstOrDefault(m => m.DeptID == dep.PID);
                    if (parentDepartment != null)
                    {
                        dep.FullDeptID = parentDepartment.FullDeptID + "." + dep.DeptID;
                    }
                }
              
                return db.SaveChanges() > 0;
            }
        }


        public static bool DepartmentDel(UserInfo userinfo,int deptId)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                Department dep = db.Department.FirstOrDefault(m => m.DeptID == deptId);
                if (dep != null)
                {
                    dep.ModifyBy = userinfo.UserID;
                    dep.UpdateTime = DateTime.Now;
                    dep.IsDel = true;
                }
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 判断部门是否能被删除
        /// 1.没有子部门
        /// 2.该部门下没有员工
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="fulldeptId"></param>
        /// <returns></returns>

        public static bool DepartmentCanDel(UserInfo userinfo, string fulldeptId)
        {
            bool canDelete = EmployeeBL.EmployeeQuery(userinfo, new QEmployee() {FullDeptID = fulldeptId,pageIndex = 0,pageSize = 1,sortField = "EmpCode" }).TotalCount == 0;
            if (canDelete)
            {
                canDelete= GetSubDepartment(userinfo,fulldeptId).Length == 0;
            }

            return canDelete;
        }

        public static MDepartment[] GetSubDepartment(UserInfo userInfo, string fullDepartId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                        SELECT * FROM base.Department t1  where IsDel=0");
        
            if (!String.IsNullOrWhiteSpace(fullDepartId))
            {
                sb.AppendFormat("AND t1.FullDeptID  like '{0}.%'", fullDepartId);
            }

            using (var db = DBHelper.NewDB())
            {
               return DBHelper<MDepartment>.ExecuteSqlToEntities(db, sb.ToString()).ToArray();
            }
        }


        public static MDepartment GetByID(int id)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                MDepartment dep = new MDepartment();
                var entity = db.Department.AsNoTracking().FirstOrDefault(m => m.DeptID == id);
                if (entity != null)
                {
                    dep.DeptID = entity.DeptID;
                    dep.ManagerId = entity.ManagerId;
                    dep.Name = entity.Name;
                    dep.SN = entity.SN;
                    dep.PID = entity.PID;
                    dep.Remark = entity.Remark;
                    if (dep.PID.HasValue)
                    {
                        entity = db.Department.AsNoTracking().FirstOrDefault(m => m.DeptID == dep.PID.Value);
                        if (entity != null) dep.PName = entity.Name;
                    }
                    if (dep.ManagerId.HasValue)
                    {
                        var employee = db.Employee.AsNoTracking().FirstOrDefault(m => m.EmpID == dep.ManagerId.Value);
                        if (employee != null) dep.ManagerName = employee.Name;
                    }
                    
                }
                return dep;
            }
        }
    }
}
