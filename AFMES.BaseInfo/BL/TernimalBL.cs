using SSLPA.BaseInfo.DTO;
using SeekerSoft.Base.DB;
using SeekerSoft.Base.DTO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.BL
{
    public class TernimalBL
    {

        /// <summary>
        /// 使用NFC卡进行登录验证
        /// </summary>
        /// <returns></returns>
        public static MTernimalLoginResult LoginValidate(string nfcid, string machine)
        {
            if (string.IsNullOrEmpty(nfcid))
                throw new Exception("NFCID不能为空");

            // 转换NFCID
            nfcid = SeekerSoft.Core.AFMES.NFCIDHelper.ToStand(nfcid);

            MTernimalLoginResult result = new MTernimalLoginResult();
            // 查询设备信息
            if (!string.IsNullOrEmpty(machine))
            {
                result.Machine = MachineBL.GetByCode(machine);
                if (result.Machine == null)
                    throw new Exception("设备信息获取失败。设备代码：" + machine);
            }
            // TODO:还未做权限校验，将来会判断当前用户是否有此设备的操作权限

            MEmployee emp = SeekerSoft.Base.BL.EmployeeBL.GetByNFCID(nfcid);
            if (emp == null || !emp.UserID.HasValue)
                throw new Exception("无此员工卡数据。NFCID：" + nfcid);
            MLoginUser user = SeekerSoft.Base.BL.UserBL.GetByUserID(emp.UserID.Value);
            if (user == null || user.IsDisabled)
                throw new Exception("登录失败，可能是未分配账号，请联系管理员");


            string terminal = "2";// 终端登陆
            UserInfo userinfo = UserCache.GetByID(emp.UserID.Value, terminal, UserType.Staff);
            if (userinfo == null)
            {
                userinfo = new UserInfo()
                {
                    Ticket = Guid.NewGuid(),
                    UserType = UserType.Staff,
                    UserID = emp.UserID.Value,
                    Terminal = terminal
                };
                UserCache.Add(userinfo);
                SeekerSoft.Base.BL.UserBL.SaveOnlineUser(userinfo);
            }
            userinfo.Name = emp.Name;
            userinfo.EmpID = emp.EmpID;

            // 每次登陆都更新用户信息，Ticket保留不变，这样可以同时登陆多个用户
            userinfo.AuthList = SeekerSoft.Base.BL.UserBL.GetAuthListByUser(userinfo.UserID);
            result.UserInfo = userinfo;

            return result;
        }

        /// <summary>
        /// 使用账号进行登录验证
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="machine">设备</param>
        /// <returns></returns>
        public static MTernimalLoginResult AccountLoginValidate(string username, string pwd, string machine)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pwd))
                throw new Exception("账号或密码不能为空。");
            MTernimalLoginResult result = new MTernimalLoginResult();
            // 查询设备信息
            if (!string.IsNullOrEmpty(machine))
            {
                result.Machine = MachineBL.GetByCode(machine);
                if (result.Machine == null)
                    throw new Exception("设备信息获取失败。设备代码：" + machine);
            }
            string terminal = "2";// 终端登陆
            LoginUser entity;
            Employee empEntity = null;
            using (var db = DBHelper.NewDB())
            {
                entity = db.LoginUser.AsNoTracking()
                                .Where(_ => !_.IsDel && !_.IsDisabled
                                    && _.LoginName == username && _.Pwd == pwd)
                                .SingleOrDefault();
                if (entity != null)
                    empEntity = db.Employee.AsNoTracking().FirstOrDefault(_ => !_.IsDel && _.Status == "0" && _.UserID == entity.UserID);
            }
            if (entity == null)
            {
                throw new Exception("登录失败，账号或密码错误");
            }
            UserInfo userinfo = UserCache.GetByID(entity.UserID, terminal, UserType.Staff);
            if (userinfo == null)
            {
                userinfo = new UserInfo()
                {
                    Ticket = Guid.NewGuid(),
                    UserType = UserType.Staff,
                    UserID = entity.UserID,
                    Terminal = terminal
                };
                UserCache.Add(userinfo);
                SeekerSoft.Base.BL.UserBL.SaveOnlineUser(userinfo);
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
            userinfo.AuthList = SeekerSoft.Base.BL.UserBL.GetAuthListByUser(userinfo.UserID);
            result.UserInfo = userinfo;
            return result;
        }


    }
}
