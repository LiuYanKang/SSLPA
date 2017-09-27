using SeekerSoft.Base.DB;
using SeekerSoft.Base.DTO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.Config;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.SqlClient;
using SeekerSoft.Core.DB;

namespace SeekerSoft.Base.BL
{
    public class UserBL
    {
        /// <summary>
        /// 根据账号密码进行登录验证
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static UserInfo LoginValidateForWebsite(string username, string pwd)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pwd))
                throw new Exception("账号或密码不能为空。");

            string terminal = "1";// 网站登陆

            UserInfo userinfo;
            LoginUser entity;
            Employee empEntity = null;
            using (var db = DB.DBHelper.NewDB())
            {
                entity = db.LoginUser.AsNoTracking()
                                .Where(_ => !_.IsDel && !_.IsDisabled
                                    && _.LoginName == username && _.Pwd == pwd)
                                .SingleOrDefault();
                if (entity != null)
                    empEntity = db.Employee.AsNoTracking().FirstOrDefault(_ => !_.IsDel && _.Status == "0" && _.UserID == entity.UserID);

                //                if (username == "admin")
                //                {
                //                }
                //                else
                //                {
                //                    // 密码加密，验证账号
                //                    string ss = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "SHA1");
                //                    string mubeausername = DBHelper<string>.ExecuteSqlToEntities(db, @"
                //SELECT TOP 1
                //        User_Name
                //FROM    V_UserInfo
                //WHERE   User_Name = @username
                //        AND User_Password = @pwd"
                //                        , new SqlParameter("username", username)
                //                        , new SqlParameter("pwd", ss))
                //                        .SingleOrDefault();
                //                    if (string.IsNullOrEmpty(mubeausername)) return null;
                //                    entity = db.LoginUser.AsNoTracking().SingleOrDefault(_ => _.LoginName == mubeausername && !_.IsDel && !_.IsDisabled);
                //                    if (entity != null)
                //                        empEntity = db.Employee.AsNoTracking().FirstOrDefault(_ => !_.IsDel && _.Status == "0" && _.UserID == entity.UserID);
                //                }
            }
            if (entity == null) return null;
            userinfo = UserCache.GetByID(entity.UserID, terminal, UserType.Staff);
            if (userinfo == null)
            {
                userinfo = new UserInfo()
                {
                    Ticket = Guid.NewGuid(),
                    UserType = UserType.Staff,
                    UserID = entity.UserID,
                    Terminal = terminal
                };
                UserCache.Add(userinfo); SaveOnlineUser(userinfo);
            }
            if (empEntity != null)
            {
                userinfo.Name = empEntity.Name;
                userinfo.EmpID = empEntity.EmpID;
                userinfo.DeptID = empEntity.DeptID;
            }
            else
            {
                userinfo.Name = entity.LoginName;
            }


            // 每次登陆都更新用户信息，Ticket保留不变，这样可以同时登陆多个用户
            userinfo.AuthList = UserBL.GetAuthListByUser(userinfo.UserID);

            // 判断是否有后台登陆授权
            if (!userinfo.AuthList.Contains("0001"))
                throw new Exception("未获得后台登陆授权，请联系管理员");

            return userinfo;
        }

        /// <summary>
        /// 根据用户标识获取用户拥有的权限
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string[] GetAuthListByUser(int userid)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                // 角色权限和用户权限的并集
                var q = (from roleFuncMap in db.RoleFuncMap.AsNoTracking()
                         join userrole in db.UserRole.AsNoTracking() on roleFuncMap.RoleID equals userrole.RoleID
                         where userrole.UserID == userid
                         select roleFuncMap.FuncCode)
                         .Union(new string[] { "Staff" });


                var department = (from m in db.Employee
                                  join d in db.Department on m.DeptID equals d.DeptID
                                  where m.UserID == userid
                                  select d).FirstOrDefault();
                if (null != department)
                {
                    var q2 = (from f in db.DepartmentFuncMap
                              where f.DepartmentID == department.DeptID
                              select f.FuncCode).ToArray().Union(q).Distinct();

                    return q2.ToArray();
                }
                else
                {
                    return q.ToArray();
                }

            }

        }


        /// <summary>
        /// 修改用户自己的密码
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns></returns>
        public static bool ChangePwd(UserInfo userinfo, string oldPwd, string newPwd)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var emp = db.LoginUser.Where(_ => _.UserID == userinfo.UserID && _.Pwd == oldPwd).FirstOrDefault();
                if (emp == null)
                    throw new Exception("原始密码错误。");

                emp.Pwd = newPwd;
                db.SaveChanges();
            }

            Logout(userinfo);

            return true;
        }

        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="userinfo">用户信息</param>
        /// <param name="models">用户实体集合</param>
        /// <returns></returns>
        public static bool SaveUser(UserInfo userinfo, List<MUser> models)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                foreach (MUser model in models)
                {
                    var m = db.LoginUser.Where(_ => _.UserID == model.UserID).FirstOrDefault();
                    if (m == null)
                    {// 新增
                        m = new LoginUser()
                        {
                            UserID = model.UserID,
                            LoginName = model.LoginName,
                            Pwd = !string.IsNullOrEmpty(ConfigHelper.Get("defaultPassword")) ? ConfigHelper.Get("defaultPassword") : "",//默认密码
                            IsDisabled = model.IsDisabled,
                            IsDel = model.IsDel
                        };
                        db.LoginUser.Add(m);
                    }
                    else
                    {// 更新
                        m.LoginName = model.LoginName;
                        m.IsDisabled = model.IsDisabled;
                    }
                }
                db.SaveChanges();
            }

            return true;
        }

        public static MLoginUser GetByUserID(int userid)
        {
            string sql = @"
SELECT  UserID ,
        LoginName ,
        IsDisabled
FROM    base.LoginUser
WHERE   IsDel = 0
        AND UserID = @userid";
            using (var db = DB.DBHelper.NewDB())
            {
                return DBHelper<MLoginUser>.ExecuteSqlToEntities(db, sql, new SqlParameter("userid", userid)).FirstOrDefault();
            }
        }

        public static bool LoginUserAdd(UserInfo userinfo, MLoginUser model)
        {
            using (var db = DBHelper.NewDB())
            {
                if (db.LoginUser.Any(_ => !_.IsDel && _.LoginName == model.LoginName))
                    throw new Exception("用户名已存在。");

                //新增
                var entity = db.LoginUser.Add(new LoginUser()
                {
                    LoginName = model.LoginName,
                    Pwd = model.Pwd,
                    IsDel = false
                });
                db.SaveChanges();

                var dbo = db.Employee.Where(_ => _.EmpID == model.EmpID).SingleOrDefault();
                dbo.UserID = entity.UserID;
                db.SaveChanges();
            }
            return true;
        }

        public static PagerResult<MUser> GetUserList(UserInfo userinfo, string loginName, PagerParams param)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" SELECT");
            sql.AppendLine(" UserID,LoginName,IsDisabled");
            sql.AppendLine(" FROM");
            sql.AppendLine(" base.LoginUser");
            sql.AppendFormat(" WHERE IsDel = 0 AND LoginName LIKE '%{0}%'", loginName.Replace("'", "''"));

            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = "LoginName";
            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                PagerResult<MUser> data = DBHelper<MUser>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        #region 删除角色信息
        public static bool DelUserItem(UserInfo userinfo, int[] userID)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var models = from dic in db.LoginUser
                             where userID.Contains(dic.UserID)
                             select dic;
                foreach (LoginUser model in models)
                {
                    model.IsDel = true;
                }
                db.SaveChanges();
            }
            return true;
        }
        #endregion

        #region 密码初始化信息
        public static bool PasswordInit(UserInfo userinfo, int[] userID)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var models = from dic in db.LoginUser
                             where userID.Contains(dic.UserID)
                             select dic;
                foreach (LoginUser model in models)
                {
                    model.Pwd = !string.IsNullOrEmpty(ConfigHelper.Get("defaultPassword")) ? ConfigHelper.Get("defaultPassword") : "123456";//默认密码;
                }
                db.SaveChanges();
            }
            return true;
        }
        #endregion

        #region 用户角色配置
        // 获取用户拥有的角色
        public static int[] GetUserRole(UserInfo userinfo, int userID)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                return db.UserRole.Where(_ => _.UserID == userID).Select(_ => _.RoleID).ToArray();
            }
        }
        public static bool SaveUserRole(UserInfo userinfo, int userID, int[] roleList)
        {
            using (var db = DBHelper.NewDB())
            {
                // 移除所有已有权限
                db.UserRole.RemoveRange(db.UserRole.Where(_ => _.UserID == userID));
                // 新增选择的权限
                db.UserRole.AddRange(roleList.Select(roleId => new UserRole()
                {
                    UserID = userID,
                    RoleID = roleId
                }));

                db.SaveChanges();
            }
            return true;
        }

        #endregion

        /// <summary>
        /// 获取当前用户拥有的菜单
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public static MMenu[] GetUserMenu(UserInfo UserInfo, string pcode = null)
        {
            using (var db = DBHelper.NewDB())
            {
                var department = db.Department.FirstOrDefault(m => m.DeptID == UserInfo.DeptID);
                StringBuilder sql;
                if (null != department)
                {
                    sql = new StringBuilder(string.Format(@"select * from (
                                                                SELECT DISTINCT t1.MenuCode,
                                                                                t1.PCode,
                                                                                t1.Name,
                                                                                t1.Icon,
                                                                                t1.Color,
                                                                                t1.Url,
                                                                                t1.SN
                                                                  FROM base.Menu t1
                                                                       INNER JOIN base.RoleMenuMap t2 ON t2.MenuCode = t1.MenuCode
                                                                       INNER JOIN base.UserRole t3 ON t3.RoleID = t2.RoleID
                                                                 WHERE t1.Visible = 1 AND t3.UserID=@userid
                                                                UNION
                                                                SELECT DISTINCT t1.MenuCode,
                                                                                t1.PCode,
                                                                                t1.Name,
                                                                                t1.Icon,
                                                                                t1.Color,
                                                                                t1.Url,
                                                                                t1.SN
                                                                  FROM base.Menu t1
                                                                       INNER JOIN base.DepartmentMenuMap t2 ON t2.MenuCode = t1.MenuCode
                                                                 WHERE t1.Visible = 1 and t2.DepartmentID='{0}') as t1 ", department.DeptID));
                    if (!string.IsNullOrEmpty(pcode))
                    {
                        sql.AppendFormat(" where t1.MenuCode LIKE '{0}%' ", pcode);
                    }
                    sql.Append(" ORDER BY t1.SN");
                }
                else
                {
                    sql = new StringBuilder(@"SELECT  DISTINCT 
	                                                            t1.MenuCode ,
	                                                            t1.PCode ,
	                                                            t1.Name ,
	                                                            t1.Icon ,
	                                                            t1.Color ,
	                                                            t1.Url ,
                                                                t1.SN
                                                            FROM base.Menu t1
                                                            INNER JOIN base.RoleMenuMap t2 ON t2.MenuCode = t1.MenuCode
                                                            INNER JOIN base.UserRole t3 ON t3.RoleID = t2.RoleID
                                                            WHERE t1.Visible=1 AND t3.UserID=@userid");
                    if (!string.IsNullOrEmpty(pcode))
                    {
                        sql.AppendFormat(" AND t1.MenuCode LIKE '{0}%' ", pcode);
                    }
                    sql.Append(" ORDER BY t1.SN");
                }


                return DBHelper<MMenu>.ExecuteSqlToEntities(db, sql.ToString(), new SqlParameter("userid", UserInfo.UserID)).ToArray();
            }
        }




        /// <summary>
        /// 保存在线用户信息到数据库
        /// </summary>
        /// <param name="userinfo"></param>
        public static void SaveOnlineUser(UserInfo userinfo)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                // 保存在线用户信息到数据库
                db.OnlineUser.Add(new OnlineUser()
                {
                    Ticket = userinfo.Ticket,
                    UserID = userinfo.UserID,
                    UserType = (int)userinfo.UserType,
                    Terminal = userinfo.Terminal,
                    BeginTime = DateTime.Now,
                    EndTime = DateTime.Now.AddYears(1),//登陆信息最长保持一年
                    IsDis = false
                });
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 初始化在线用户信息
        /// 从数据库中还原在线用户信息
        /// 在系统启动时运行
        /// </summary>
        public static void InitOnlineUser()
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var userlist = db.OnlineUser.AsNoTracking()
                    .Where(_ => !_.IsDis && _.EndTime > DateTime.Now)
                    .Select(_ => new UserInfo()
                    {
                        Ticket = _.Ticket,
                        UserID = _.UserID,
                        UserType = (UserType)_.UserType,
                        Terminal = _.Terminal
                    }).ToArray();

                foreach (var li in userlist)
                {
                    if (li.UserType == UserType.Staff)
                    {
                        // 恢复权限
                        li.AuthList = UserBL.GetAuthListByUser(li.UserID);
                        // 恢复姓名、职员ID
                        var empEntity = db.Employee.AsNoTracking().FirstOrDefault(_ => !_.IsDel && _.Status == "0" && _.UserID == li.UserID);
                        if (empEntity != null)
                        {
                            li.Name = empEntity.Name;
                            li.EmpID = empEntity.EmpID;
                        }
                    }

                    UserCache.Add(li);
                }
            }
        }

        /// <summary>
        /// 登出当前用户
        /// </summary>
        /// <param name="userinfo"></param>
        public static bool Logout(UserInfo userinfo)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var onlineuser = db.OnlineUser.Single(_ => _.Ticket == userinfo.Ticket);
                onlineuser.IsDis = true;
                db.SaveChanges();
            }
            return UserCache.RemoveByTicket(userinfo.Ticket);
        }



        /// <summary>
        /// 慕贝尔用户查询
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static PagerResult<MMubeaUser> QueryMubeaUser(UserInfo userinfo, QMubeaUser param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@" SELECT  User_Name ,
        User_Password,
        Email
FROM    V_UserInfo ");
            if (!string.IsNullOrEmpty(param.Keyword))
                sql.AppendFormat(" WHERE User_Name LIKE '%{0}%'", param.Keyword.Replace("'", "''"));

            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = "User_Name";
            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                return DBHelper<MMubeaUser>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
            }
        }
    }
}
