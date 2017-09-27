using SSLPA.BaseInfo.BL;
using SSLPA.BaseInfo.DTO;
using SeekerSoft.Core.ServiceModel;
using SeekerSoft.Core.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace SSLPA.BaseInfo.Srv
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AuthValidService]
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    public class BaseInfoSrv : ServiceBase
    {
        [WebGet]
        [OperationContract]
        public string test()
        {
            return "调用成功";
        }



        // 手持终端NFC卡登陆
        [WebGet]
        [OperationContract]
        public SrvResult<MTernimalLoginResult> LoginValidateForTernimal(string nfcid, string machine)
        {
            var result = new SrvResult<MTernimalLoginResult>();
            result.Data = TernimalBL.LoginValidate(nfcid, machine);
            result.Status = ResultStatus.Success;
            return result;
        }

        // 手持终端账号密码登陆
        [WebGet]
        [OperationContract]
        public SrvResult<MTernimalLoginResult> AccountLoginValidateForTernimal(string username, string pwd, string machine)
        {
            var result = new SrvResult<MTernimalLoginResult>();
            result.Data = TernimalBL.AccountLoginValidate(username, pwd, machine);
            result.Status = ResultStatus.Success;
            return result;
        }

        #region 工序
        //查询数据
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MWorkProcess>> WorkProcessList(Guid ticket, QWorkProcess pagerParams)
        {
            return new SrvResult<PagerResult<MWorkProcess>>()
            {
                Data = WorkProcessBL.WorkProcessList(UserInfo, pagerParams),
                Status = ResultStatus.Success
            };
        }
        //获取详细信息
        [Auth]
        [OperationContract]
        public SrvResult<MWorkProcess> WorkProcessGet(Guid ticket, int id)
        {
            return new SrvResult<MWorkProcess>()
            {
                Data = WorkProcessBL.WorkProcessGet(UserInfo, id),
                Status = ResultStatus.Success
            };
        }
        //新增
        [Auth]
        [OperationContract]
        public SrvResult<bool> WorkProcessAdd(Guid ticket, MWorkProcess model)
        {
            return new SrvResult<bool>()
            {
                Data = WorkProcessBL.WorkProcessAdd(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        //保存
        [Auth]
        [OperationContract]
        public SrvResult<bool> WorkProcessSave(Guid ticket, MWorkProcess model)
        {
            return new SrvResult<bool>()
            {
                Data = WorkProcessBL.WorkProcessSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        //删除
        [Auth]
        [OperationContract]
        public SrvResult<bool> WorkProcessDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = WorkProcessBL.WorkProcessDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }

        //根据物料类型 查询工序
        [Auth]
        [OperationContract]
        public SrvResult<MWorkProcess[]> WorkProcessListByMaterialID(Guid ticket, string type)
        {
            return new SrvResult<MWorkProcess[]>()
            {
                Data = WorkProcessBL.WorkProcessListByMaterialID(UserInfo, type),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<MWorkProcess[]> WorkProcessAllS(Guid ticket)
        {
            return new SrvResult<MWorkProcess[]>()
            {
                Data = WorkProcessBL.QueryAll(),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 获取产品部门工序列表
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="param"></param>
        /// <param name="pDCode"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MWorkProcess>> GetWorkPressList(Guid ticket, QWorkProcess param, string pDCode)
        {
            return new SrvResult<PagerResult<MWorkProcess>>()
            {
                Data = WorkProcessBL.GetWorkPressList(UserInfo, param, pDCode),
                Status = ResultStatus.Success
            };
        }

        #endregion




        #region 设备
        //查询数据
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MMachine>> MachineQuery(Guid ticket, QMachine pagerParams)
        {
            return new SrvResult<PagerResult<MMachine>>()
            {
                Data = MachineBL.MachineQuery(UserInfo, pagerParams),
                Status = ResultStatus.Success
            };
        }
        //获取详细信息
        [Auth]
        [OperationContract]
        public SrvResult<MMachine> MachineGet(Guid ticket, int id)
        {
            return new SrvResult<MMachine>()
            {
                Data = MachineBL.MachineGet(UserInfo, id),
                Status = ResultStatus.Success
            };
        }
        //新增
        [Auth]
        [OperationContract]
        public SrvResult<bool> MachineAdd(Guid ticket, MMachine model)
        {
            return new SrvResult<bool>()
            {
                Data = MachineBL.MachineAdd(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        //保存
        [Auth]
        [OperationContract]
        public SrvResult<bool> MachineSave(Guid ticket, MMachine model)
        {
            return new SrvResult<bool>()
            {
                Data = MachineBL.MachineSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        //保存
        [Auth]
        [OperationContract]
        public SrvResult<bool> MachineQualityWarningSave(Guid ticket, MMachine model)
        {
            return new SrvResult<bool>()
            {
                Data = MachineBL.MachineQualityWarningSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        //删除
        [Auth]
        [OperationContract]
        public SrvResult<bool> MachineDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = MachineBL.MachineDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }

        //查询所有设备
        [Auth]
        [OperationContract]
        public SrvResult<MMachine[]> MachineAllS(Guid ticket,int id)
        {
            return new SrvResult<MMachine[]>()
            {
                Data = MachineBL.QueryAll(id),
                Status = ResultStatus.Success
            };
        }

        //获取设备状态信息(半成品生产设备)
        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MMachineState> GetMachineStateSemi(Guid ticket, string machine)
        {
            return new SrvResult<MMachineState>()
            {
                Data = MachineBL.GetMachineStateSemi(UserInfo, machine),
                Status = ResultStatus.Success
            };
        }

        //获取设备状态信息（产品生产设备）
        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MMachineState> GetMachineStateProd(Guid ticket, string machine)
        {
            return new SrvResult<MMachineState>()
            {
                Data = MachineBL.GetMachineStateProd(UserInfo, machine),
                Status = ResultStatus.Success
            };
        }

        //获取设备停机状态信息
        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MMachineDown> GetMachineDownInfo(Guid ticket, string machineCode)
        {
            return new SrvResult<MMachineDown>()
            {
                Data = MachineBL.GetMachineDownInfo(UserInfo, machineCode),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 在EWI终端 查询设备停机记录
        /// </summary>
        [OperationContract]
        public SrvResult<PagerResult<MMachineDown>> QueryMachineDownByEWI(int machineid, PagerParams param)
        {
            return new SrvResult<PagerResult<MMachineDown>>()
            {
                Data = MachineBL.QueryCloseDownByEWI(machineid, param),
                Status = ResultStatus.Success
            };
        }
        /// <summary>
        /// 根据产线ID获取设备
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="productLineId"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<MMachine[]> GetLineMachineList(Guid ticket, int productLineId)
        {
            return new SrvResult<MMachine[]>()
            {
                Data = MachineBL.GetLineMachineList(productLineId),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 根据产线ID获取设备
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="procId"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<MMachine[]> GetWorkMachineList(Guid ticket, int procId)
        {
            return new SrvResult<MMachine[]>()
            {
                Data = MachineBL.GetWorkMachineList(procId),
                Status = ResultStatus.Success
            };
        }

        #endregion



        #region 天气
        //获取当前天气
        [WebGet]
        [OperationContract]
        public SrvResult<MWeather> QueryWeather()
        {
            return new SrvResult<MWeather>()
            {
                Data = WeatherBL.Query(),
                Status = ResultStatus.Success
            };
        }
        #endregion
        

        #region EHS
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MEHS>> EHSQuery(Guid ticket, string keyword, PagerParams param)
        {
            return new SrvResult<PagerResult<MEHS>>()
            {
                Data = EHSBL.Query(keyword, param),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<MEHS> EHSGet(Guid ticket, int id)
        {
            return new SrvResult<MEHS>()
            {
                Data = EHSBL.Get(id),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> EHSAdd(Guid ticket, MEHS model)
        {
            return new SrvResult<bool>()
            {
                Data = EHSBL.Add(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> EHSSave(Guid ticket, MEHS model)
        {
            return new SrvResult<bool>()
            {
                Data = EHSBL.Save(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> EHSDel(Guid ticket, int[] ids)
        {
            return new SrvResult<bool>()
            {
                Data = EHSBL.Del(UserInfo, ids),
                Status = ResultStatus.Success
            };
        }

        [WebGet]
        [OperationContract]
        public SrvResult<MEHS[]> GetMachineEHS(int machineid)
        {
            return new SrvResult<MEHS[]>()
            {
                Data = EHSBL.GetMachineEHS(machineid),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> SaveMachineEHS(Guid ticket, int machineid, int[] ids)
        {
            return new SrvResult<bool>()
            {
                Data = EHSBL.SaveMachineEHS(machineid, ids),
                Status = ResultStatus.Success
            };
        }
        #endregion
        
        #region 产品部门维护

        /// <summary>
        /// 查询产品部门
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MProductDept>> ProductDeptList(Guid ticket, QProductDept param)
        {
            return new SrvResult<PagerResult<MProductDept>>()
            {
                Data = ProductDeptBL.ProductDeptList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 查询某一个产品部门
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="pdcode"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<MProductDept> ProductDeptGet(Guid ticket, string pdcode)
        {
            return new SrvResult<MProductDept>()
            {
                Data = ProductDeptBL.ProductDeptGet(pdcode),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 新增/编辑产品部门
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<bool> ProductDeptSave(Guid ticket, MProductDept model)
        {
            return new SrvResult<bool>()
            {
                Data = ProductDeptBL.ProductDeptSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 删除产品部门
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="pdcode"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<bool> ProductDeptDel(Guid ticket, string  pdcode)
        {
            return new SrvResult<bool>()
            {
                Data = ProductDeptBL.ProductDeptDel(UserInfo, pdcode),
                Status = ResultStatus.Success
            };
        }
        #endregion


        #region 产线维护

        /// <summary>
        /// 查询产线
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MProductLine>> ProductLineList(Guid ticket, QProductLine param)
        {
            return new SrvResult<PagerResult<MProductLine>>()
            {
                Data = ProductLineBL.ProductLineList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 查询某一个产线
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<MProductLine> ProductLineGet(Guid ticket, int id)
        {
            return new SrvResult<MProductLine>()
            {
                Data = ProductLineBL.ProductLineGet(id),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 获取产品部门表数据
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<MProductDept[]> ProDept(Guid ticket)
        {
            return new SrvResult<MProductDept[]>()
            {
                Data = ProductLineBL.ProDept(),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 新增/编辑产品部门
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<bool> ProductLineSave(Guid ticket, MProductLine model)
        {
            return new SrvResult<bool>()
            {
                Data = ProductLineBL.ProductLineSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 拖拽之后保存所有数据
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<bool> ProductLineSnSave(Guid ticket, int[] idList)
        {
            return new SrvResult<bool>()
            {
                Data = ProductLineBL.ProductLineSnSave(UserInfo, idList),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 删除产线
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<bool> ProductLineDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = ProductLineBL.ProductLineDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MProductLine>> GetProductLineList(Guid ticket, QProductLine param,string pDCode)
        {
            return new SrvResult<PagerResult<MProductLine>>()
            {
                Data = ProductLineBL.GetProductLineList(UserInfo, param, pDCode),
                Status = ResultStatus.Success
            };
        }

        #endregion




    }
}
