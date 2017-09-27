using SeekerSoft.Base.DTO;
using SeekerSoft.Core.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeekerSoft.Core.ServiceModel;
using SeekerSoft.Base.DB;
using SeekerSoft.Core.DB;

namespace SeekerSoft.Base.BL
{
    public class RoleBL
    {
        public static MRole[] RoleAll()
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var q = from dic in db.Role.AsNoTracking()
                        select new MRole()
                        {
                            RoleID = dic.RoleID,
                            Name = dic.Name,
                            Remark = dic.Remark
                        };

                return q.ToArray();
            }
        }

        public static MFunc[] FuncAll()
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var q = from func in db.FunctionDefine.AsNoTracking()
                        orderby func.FuncCode
                        select new MFunc()
                        {
                            FuncCode = func.FuncCode,
                            ParentCode = func.ParentCode,
                            Name = func.Name,
                            Remark = func.Remark
                        };

                return q.ToArray();
            }
        }

        #region 角色查询
        //public static MRole[] RoleItems(string name)
        //{
        //    using (var db = DB.BaseDB.NewDB())
        //    {
        //        var q = from dic in db.Role.AsNoTracking()
        //                where dic.Name.Contains(name)
        //                select new MRole()
        //                {
        //                    RoleID = dic.RoleID,
        //                    Name = dic.Name,
        //                    Remark = dic.Remark
        //                };

        //        return q.ToArray();
        //    }
        //}
        public static PagerResult<MRole> GetRoleList(UserInfo userinfo, QRole param)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT  RoleID ,
        Name ,
        Remark FROM base.Role
        WHERE 1=1");
            //查询条件
            if (!string.IsNullOrWhiteSpace(param.Keyword))
            {
                sb.AppendFormat(@"and Name like '%{0}%' ", param.Keyword.Replace("'", "''"));
            }

            //排序
            if (string.IsNullOrEmpty(param.sortField))
            {
                param.sortField = "RoleID";
                param.sortOrder = "ASC";
            }
            sb.AppendFormat("order by {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));      //按排序字段，排序类型

            SqlPageProcedureParams spp = new SqlPageProcedureParams();
            spp.pageSize = param.pageSize;
            spp.pageIndex = param.pageIndex;
            spp.sql = sb.ToString();
            using (var db = DBHelper.NewDB())
            {
                PagerResult<MRole> data = DBHelper<MRole>.ExecuteSqlPageProcedureToEntities(db, spp);
                return data;
            }
        }
        #endregion

        #region 保存角色信息
        public static bool SavaRoleList(UserInfo userinfo, List<MRole> models)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                //var m = db.Role.Where(_ => _.RoleID == model.RoleID).FirstOrDefault();
                //if (m == null)
                //{// 新增
                //    m = new DB.Role()
                //    {
                //        RoleID = model.RoleID,
                //        Name = model.Name,
                //        Remark = model.Remark
                //    };
                //    db.Role.Add(m);
                //}
                //else
                //{// 更新
                //    m.Name = model.Name;
                //    m.Remark = model.Remark;
                //}
                foreach (MRole model in models)
                {
                    var m = db.Role.Where(_ => _.RoleID == model.RoleID).FirstOrDefault();
                    if (m == null)
                    {// 新增
                        m = new Role()
                        {
                            RoleID = model.RoleID,
                            Name = model.Name,
                            Remark = model.Remark
                        };
                        db.Role.Add(m);
                    }
                    else
                    {// 更新
                        m.Name = model.Name;
                        m.Remark = model.Remark;
                    }
                }
                db.SaveChanges();
            }
            return true;
        }
        public static bool SaveFunc(int roleID, string[] funcList)
        {
            using (var db = DBHelper.NewDB())
            {
                // 移除所有已有权限
                db.RoleFuncMap.RemoveRange(db.RoleFuncMap.Where(_ => _.RoleID == roleID));
                // 新增选择的权限
                db.RoleFuncMap.AddRange(funcList.Select(func => new RoleFuncMap()
                {
                    RoleID = roleID,
                    FuncCode = func
                }));

                db.SaveChanges();
            }
            return true;
        }

        public static bool SaveDepartmentFunc(int departmentId, string[] funcList)
        {
            using (var db = DBHelper.NewDB())
            {
                // 移除所有已有权限
                db.DepartmentFuncMap.RemoveRange(db.DepartmentFuncMap.Where(_ => _.DepartmentID == departmentId));
                // 新增选择的权限
                db.DepartmentFuncMap.AddRange(funcList.Select(func => new DepartmentFuncMap()
                {
                    DepartmentID = departmentId,
                    FuncCode = func
                }));

                db.SaveChanges();
            }
            return true;
        }


        // 获取角色映射的所有权限
        public static string[] GetFunc(int roleID)
        {
            using (var db = DBHelper.NewDB())
            {
                return db.RoleFuncMap.Where(_ => _.RoleID == roleID).Select(_ => _.FuncCode).ToArray();
            }

        }

        // 获取角色映射的所有权限
        public static string[] GetFuncByDepartmentId(int deptId)
        {
            using (var db = DBHelper.NewDB())
            {
                var result = (from f in db.DepartmentFuncMap
                              where f.DepartmentID == deptId
                              select f.FuncCode).ToArray();
                return result;
            }
        }
        #endregion

        #region 删除角色信息
        public static bool DelRoleItem(UserInfo userinfo, int[] roleID)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var models = from dic in db.Role
                             where roleID.Contains(dic.RoleID)
                             select dic;
                db.Role.RemoveRange(models.ToList());
                db.SaveChanges();
            }
            return true;
        }
        #endregion


        // 获取角色映射的所有菜单
        public static string[] GetMenu(int roleID)
        {
            using (var db = DBHelper.NewDB())
            {
                return db.RoleMenuMap.Where(_ => _.RoleID == roleID).Select(_ => _.MenuCode).ToArray();
            }

        }

        public static string[] GetMenuByDepartmentId(int deptId)
        {
            using (var db = DBHelper.NewDB())
            {
                var result = (from f in db.DepartmentMenuMap
                              where f.DepartmentID == deptId
                              select f.MenuCode).ToArray();
                return result;
            }

        }

        public static bool SaveMenu(int roleID, string[] menuList)
        {
            using (var db = DBHelper.NewDB())
            {
                // 移除所有已有权限
                db.RoleMenuMap.RemoveRange(db.RoleMenuMap.Where(_ => _.RoleID == roleID));
                // 新增选择的权限
                db.RoleMenuMap.AddRange(menuList.Select(menu => new RoleMenuMap()
                {
                    RoleID = roleID,
                    MenuCode = menu
                }));

                db.SaveChanges();
            }
            return true;
        }


        public static bool SaveDepartmentMenu(int departmentId, string[] menuList)
        {
            using (var db = DBHelper.NewDB())
            {
                // 移除所有已有权限
                db.DepartmentMenuMap.RemoveRange(db.DepartmentMenuMap.Where(_ => _.DepartmentID == departmentId));
                // 新增选择的权限
                db.DepartmentMenuMap.AddRange(menuList.Select(menu => new DepartmentMenuMap()
                {
                    DepartmentID = departmentId,
                    MenuCode = menu
                }));

                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 根据用户ID获取当前员工的所属部门（为空时默认最高权限）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetPdCode(int userId)
        {
            var pdCode = string.Empty;
            using (var db = DBHelper.NewDB())
            {
                var emp = db.Employee.SingleOrDefault(x => x.UserID == userId);
                if (emp != null)
                {
                    pdCode = emp.PDCode;
                }
            }
            return pdCode;
        }

    }
}
