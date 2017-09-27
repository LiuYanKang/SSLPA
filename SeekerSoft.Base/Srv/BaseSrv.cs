using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using SeekerSoft.Core.WCF;
using SeekerSoft.Core.ServiceModel;
using SeekerSoft.Core.Auth;
using SeekerSoft.Base.BL;
using SeekerSoft.Base.DTO;

namespace SeekerSoft.Base.Srv
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AuthValidService]
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BaseSrv : ServiceBase
    {
        [WebGet]
        [OperationContract]
        public string test()
        {
            return "调用成功";
        }

        [OperationContract]
        public SrvResult<string> GetLicense()
        {
            return new SrvResult<string>()
            {
                Data = Core.Config.ConfigHelper.Get("License"),
                Status = ResultStatus.Success
            };
        }

        #region 登陆
        [OperationContract]
        public SrvResult<UserInfo> LoginValidate(string username, string pwd)
        {
            var result = new SrvResult<UserInfo>();
            result.Data = UserBL.LoginValidateForWebsite(username, pwd);
            result.Status = ResultStatus.Success;
            return result;
        }

        [WebGet]
        [OperationContract]
        public SrvResult<bool> HasLogin(Guid ticket)
        {
            return new SrvResult<bool>()
            {
                Data = UserInfo != null,
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 登出当前用户
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<bool> Logout(Guid ticket)
        {
            return new SrvResult<bool>()
            {
                Data = UserBL.Logout(UserInfo),
                Status = ResultStatus.Success
            };
        }

        [Auth("Staff")]
        [OperationContract]
        public SrvResult<bool> ChangePwd(Guid ticket, string oldPwd, string newPwd)
        {
            return new SrvResult<bool>()
            {
                Data = UserBL.ChangePwd(UserInfo, oldPwd, newPwd),
                Status = ResultStatus.Success
            };
        }


        [Auth("Staff")]
        [OperationContract]
        public SrvResult<MMenu[]> GetUserMenu(Guid ticket, string pcode)
        {
            return new SrvResult<MMenu[]>()
            {
                Data = UserBL.GetUserMenu(UserInfo, pcode),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region 数据字典

        //查询获取字典数据
        [Auth]
        [OperationContract]
        public SrvResult<MDic[]> DicAll(Guid ticket, QDic pagerParams)
        {
            return new SrvResult<MDic[]>()
            {
                Data = DicBL.DicAll(UserInfo, pagerParams),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<MDicItem[]> QueryDicItems(Guid ticket, string dicCode)
        {
            return new SrvResult<MDicItem[]>()
            {
                Data = DicBL.DicItems(dicCode),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveDicItem(Guid ticket, MDicItem model)
        {
            return new SrvResult<bool>()
            {
                Data = DicBL.SaveDicItem(UserInfo, model),
                Status = ResultStatus.Success
            };
        }


        [Auth]
        [OperationContract]
        public SrvResult<bool> DelDicItem(Guid ticket, string dicCode, string code)
        {
            return new SrvResult<bool>()
            {
                Data = DicBL.DelDicItem(UserInfo, dicCode, code),
                Status = ResultStatus.Success
            };
        }
        #endregion

        #region 用户管理
        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveUser(Guid ticket, List<MUser> models)
        {
            return new SrvResult<bool>()
            {
                Data = UserBL.SaveUser(UserInfo, models),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MUser>> GetUserList(Guid ticket, string loginName, PagerParams pagerParams)
        {
            return new SrvResult<PagerResult<MUser>>()
            {
                Data = UserBL.GetUserList(UserInfo, loginName, pagerParams),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<int[]> GetUserRole(Guid ticket, int userID)
        {
            return new SrvResult<int[]>()
            {
                Data = UserBL.GetUserRole(UserInfo, userID),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveUserRole(Guid ticket, int userID, int[] roleList)
        {
            return new SrvResult<bool>()
            {
                Data = UserBL.SaveUserRole(UserInfo, userID, roleList),
                Status = ResultStatus.Success
            };
        }

        // 删除用户信息
        [Auth]
        [OperationContract]
        public SrvResult<bool> DelUserItem(Guid ticket, int[] userID)
        {
            return new SrvResult<bool>()
            {
                Data = UserBL.DelUserItem(UserInfo, userID),
                Status = ResultStatus.Success
            };
        }

        // 密码初始化
        [Auth]
        [OperationContract]
        public SrvResult<bool> UserPasswordInit(Guid ticket, int[] userID)
        {
            return new SrvResult<bool>()
            {
                Data = UserBL.PasswordInit(UserInfo, userID),
                Status = ResultStatus.Success
            };
        }
        #endregion


        #region 角色
        [Auth]
        [OperationContract]
        public SrvResult<MRole[]> RoleAll(Guid ticket)
        {
            return new SrvResult<MRole[]>()
            {
                Data = RoleBL.RoleAll(),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<MFunc[]> FuncAll(Guid ticket)
        {
            return new SrvResult<MFunc[]>()
            {
                Data = RoleBL.FuncAll(),
                Status = ResultStatus.Success
            };
        }

        #region 角色查询
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MRole>> QueryRoleItems(Guid ticket, QRole param)
        {
            return new SrvResult<PagerResult<MRole>>()
            {
                Data = RoleBL.GetRoleList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<string[]> GetFuncByRole(Guid ticket, int id)
        {
            return new SrvResult<string[]>()
            {
                Data = RoleBL.GetFunc(id),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<string[]> GetMenuByRole(Guid ticket, int id)
        {
            return new SrvResult<string[]>()
            {
                Data = RoleBL.GetMenu(id),
                Status = ResultStatus.Success
            };
        }


        #endregion

        #region 角色保存
        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveRoleItem(Guid ticket, List<MRole> models)
        {
            return new SrvResult<bool>()
            {
                Data = RoleBL.SavaRoleList(UserInfo, models),
                Status = ResultStatus.Success
            };
        }

        // 保存角色拥有的权限
        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveRoleFunc(Guid ticket, int roleID, string[] funcList)
        {
            return new SrvResult<bool>()
            {
                Data = RoleBL.SaveFunc(roleID, funcList),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveRoleMenu(Guid ticket, int roleID, string[] menuList)
        {
            return new SrvResult<bool>()
            {
                Data = RoleBL.SaveMenu(roleID, menuList),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region 删除角色信息
        [Auth]
        [OperationContract]
        public SrvResult<bool> DelRoleItem(Guid ticket, int[] roleID)
        {
            return new SrvResult<bool>()
            {
                Data = RoleBL.DelRoleItem(UserInfo, roleID),
                Status = ResultStatus.Success
            };
        }
        #endregion

        #endregion

        #region 菜单管理
        [Auth]
        [OperationContract]
        public SrvResult<bool> MenuAdd(Guid ticket, MMenu model)
        {
            return new SrvResult<bool>()
            {
                Data = MenuBL.Add(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> MenuSave(Guid ticket, MMenu model)
        {
            return new SrvResult<bool>()
            {
                Data = MenuBL.Save(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<MMenu> MentGetEntity(Guid ticket, string MenuCode)
        {
            return new SrvResult<MMenu>()
            {
                Data = MenuBL.GetByCode(MenuCode),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> MenuDel(Guid ticket, string id)
        {
            return new SrvResult<bool>()
            {
                Data = MenuBL.Delete(id),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<MMenu[]> QueryMenuAll(Guid ticket)
        {
            return new SrvResult<MMenu[]>()
            {
                Data = MenuBL.QueryAll(),
                Status = ResultStatus.Success
            };
        }
        #endregion

        #region 职员管理
        //查询职员信息
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MEmployee>> EmployeeQuery(Guid ticket, QEmployee pagerParams)
        {
            return new SrvResult<PagerResult<MEmployee>>()
            {
                Data = EmployeeBL.EmployeeQuery(UserInfo, pagerParams),
                Status = ResultStatus.Success
            };
        }

        //获取职员数据
        [Auth]
        [OperationContract]
        public SrvResult<MEmployee> EmployeeGet(Guid ticket, int id)
        {
            return new SrvResult<MEmployee>()
            {
                Data = EmployeeBL.GetByID(id),
                Status = ResultStatus.Success
            };
        }

        //新增职员
        [Auth]
        [OperationContract]
        public SrvResult<bool> EmployeeAdd(Guid ticket, MEmployee model)
        {
            return new SrvResult<bool>()
            {
                Data = EmployeeBL.EmployeeAdd(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        //保存职员
        [Auth]
        [OperationContract]
        public SrvResult<bool> EmployeeSave(Guid ticket, MEmployee model)
        {
            return new SrvResult<bool>()
            {
                Data = EmployeeBL.EmployeeSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        //设置在职离职
        [Auth]
        [OperationContract]
        public SrvResult<bool> SetDisabledEmployee(Guid ticket, int id, bool isDisabled)
        {
            return new SrvResult<bool>()
            {
                Data = EmployeeBL.SetDisabledEmployee(UserInfo, id, isDisabled),
                Status = ResultStatus.Success
            };
        }

        //查询所属部门下拉框里的数据
        [Auth]
        [OperationContract]
        public SrvResult<MDepartment[]> GetDepartmentName(Guid ticket)
        {
            return new SrvResult<MDepartment[]>()
            {
                Data = EmployeeBL.GetDepartmentName(UserInfo),
                Status = ResultStatus.Success
            };
        }



        //查询数据
        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MEmployee[]> GetRepairPerson(Guid ticket)
        {
            return new SrvResult<MEmployee[]>()
            {
                Data = EmployeeBL.GetRepairPerson(UserInfo),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region 登陆账号管理

        //新增登陆账号
        [Auth]
        [OperationContract]
        public SrvResult<bool> LoginUserAdd(Guid ticket, MLoginUser model)
        {
            return new SrvResult<bool>()
            {
                Data = UserBL.LoginUserAdd(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region 组织结构管理

        //慕贝尔部门
        [Auth]
        [OperationContract]
        public SrvResult<MDepartment[]> MDepartmentAll(Guid ticket)
        {
            return new SrvResult<MDepartment[]>()
            {
                Data = DepartmentBL.MDepartmentAll(),
                Status = ResultStatus.Success
            };
        }


        //新增部门
        [Auth]
        [OperationContract]
        public SrvResult<bool> DepartmentAdd(Guid ticket, MDepartment model)
        {
            return new SrvResult<bool>()
            {
                Data = DepartmentBL.DepartmentAdd(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        //修改部门
        [Auth]
        [OperationContract]
        public SrvResult<bool> DepartmentSave(Guid ticket, MDepartment model)
        {
            return new SrvResult<bool>()
            {
                Data = DepartmentBL.DepartmentSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        //删除部门
        [Auth]
        [OperationContract]
        public SrvResult<bool> DepartmentDel(Guid ticket, int deptId)
        {
            return new SrvResult<bool>()
            {
                Data = DepartmentBL.DepartmentDel(UserInfo, deptId),
                Status = ResultStatus.Success
            };
        }

        //获取部门数据
        [Auth]
        [OperationContract]
        public SrvResult<MDepartment> DepartmentGet(Guid ticket, int id)
        {
            return new SrvResult<MDepartment>()
            {
                Data = DepartmentBL.GetByID(id),
                Status = ResultStatus.Success
            };
        }

        //删除部门
        [Auth]
        [OperationContract]
        public SrvResult<bool> DepartmentCanDel(Guid ticket, string fulldeptId)
        {
            return new SrvResult<bool>()
            {
                Data = DepartmentBL.DepartmentCanDel(UserInfo, fulldeptId),
                Status = ResultStatus.Success
            };
        }


        [Auth]
        [OperationContract]
        public SrvResult<string[]> GetFuncByDepartmentId(Guid ticket, int deptId)
        {
            return new SrvResult<string[]>()
            {
                Data = RoleBL.GetFuncByDepartmentId(deptId),
                Status = ResultStatus.Success
            };
        }


        [Auth]
        [OperationContract]
        public SrvResult<string[]> GetMenuByDepartmentId(Guid ticket, int deptId)
        {
            return new SrvResult<string[]>()
            {
                Data = RoleBL.GetMenuByDepartmentId(deptId),
                Status = ResultStatus.Success
            };
        }

        // 保存角色拥有的权限
        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveDepartmentFunc(Guid ticket, int departmentId, string[] funcList)
        {
            return new SrvResult<bool>()
            {
                Data = RoleBL.SaveDepartmentFunc(departmentId, funcList),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveDepartmentMenu(Guid ticket, int departmentId, string[] menuList)
        {
            return new SrvResult<bool>()
            {
                Data = RoleBL.SaveDepartmentMenu(departmentId, menuList),
                Status = ResultStatus.Success
            };
        }

        #endregion


        //慕贝尔用户查询
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MMubeaUser>> QueryMubeaUser(Guid ticket, QMubeaUser pagerParams)
        {
            return new SrvResult<PagerResult<MMubeaUser>>()
            {
                Data = UserBL.QueryMubeaUser(UserInfo, pagerParams),
                Status = ResultStatus.Success
            };
        }


        #region 短信发送记录

        //查询短信发送数据
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MSMSSend>> SMSSendQuery(Guid ticket, QSMSSend param)
        {
            return new SrvResult<PagerResult<MSMSSend>>()
            {
                Data = SMSProvider.SMSSendQuery(UserInfo, param),
                Status = ResultStatus.Success
            };
        }

        //短信发送详细信息
        [Auth]
        [OperationContract]
        public SrvResult<MSMSSend> SMSSendGet(Guid ticket, int id)
        {
            return new SrvResult<MSMSSend>()
            {
                Data = SMSProvider.SMSSendGet(id),
                Status = ResultStatus.Success
            };
        }
        #endregion
    }
}
