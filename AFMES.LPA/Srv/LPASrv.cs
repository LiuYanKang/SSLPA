using SSLPA.LPA.BL;
using SSLPA.LPA.DTO;
using SeekerSoft.Core.ServiceModel;
using SeekerSoft.Core.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace SSLPA.LPA.Srv
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AuthValidService]
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LPASrv : ServiceBase
    {
        [WebGet]
        [OperationContract]
        public string test()
        {
            return "调用成功";
        }

        #region 审核项目

        [Auth]
        [OperationContract]
        public SrvResult<MAuditItem> AuditItemGet(Guid ticket, int id)
        {
            return new SrvResult<MAuditItem>()
            {
                Data = AuditItemBL.AuditItemGet(id),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> AuditItemAdd(Guid ticket, MAuditItem model)
        {
            return new SrvResult<bool>()
            {
                Data = AuditItemBL.AuditItemAdd(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> AuditItemSave(Guid ticket, MAuditItem model)
        {
            return new SrvResult<bool>()
            {
                Data = AuditItemBL.AuditItemSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<MAuditItem[]> AuditItemList(Guid ticket, QAuditItem param)
        {
            return new SrvResult<MAuditItem[]>()
            {
                Data = AuditItemBL.AuditItemList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> AuditItemDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = AuditItemBL.AuditItemDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }

        //拖拽之后保存所有数据
        [Auth]
        [OperationContract]
        public SrvResult<bool> AuditItemSNSave(Guid ticket, int[] idList)
        {
            return new SrvResult<bool>()
            {
                Data = AuditItemBL.AuditItemSNSave(UserInfo, idList),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 获取相应审核项目的设备和其他
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<MAuditMachine[]> GetAuditMachine(Guid ticket, int id = 0)
        {
            return new SrvResult<MAuditMachine[]>()
            {
                Data = AuditItemBL.GetAuditMachine(id),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region LPA员工信息

        [Auth]
        [OperationContract]
        public SrvResult<MEmployee> EmployeeGet(Guid ticket, int id)
        {
            return new SrvResult<MEmployee>()
            {
                Data = EmployeeBL.EmployeeGet(id),
                Status = ResultStatus.Success
            };
        }
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
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MEmployee>> EmployeeList(Guid ticket, QEmployee param)
        {
            return new SrvResult<PagerResult<MEmployee>>()
            {
                Data = EmployeeBL.EmployeeList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> EmployeeDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = EmployeeBL.EmployeeDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }


        [Auth]
        [WebGet]
        [OperationContract]
        public SrvResult<MArea[]> EmployeeAuditArea(Guid ticket, int planId)
        {
            return new SrvResult<MArea[]>()
            {
                Data = EmployeeBL.GetAuditArea(planId),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region LPA执行计划

        [Auth]
        [OperationContract]
        public SrvResult<MActionPlan2> ActionPlanGet(Guid ticket, int id)
        {
            return new SrvResult<MActionPlan2>()
            {
                Data = ActionPlanBL2.ActionPlanGet(id),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> ActionPlanAdd(Guid ticket, MActionPlan2 model)
        {
            return new SrvResult<bool>()
            {
                Data = ActionPlanBL2.ActionPlanAdd(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        //[Auth]
        //[OperationContract]
        //public SrvResult<bool> ActionPlanSave(Guid ticket, MActionPlan2 model)
        //{
        //    return new SrvResult<bool>()
        //    {
        //        Data = ActionPlanBL2.ActionPlanSave(UserInfo, model),
        //        Status = ResultStatus.Success
        //    };
        //}
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MActionPlan2>> ActionPlanList(Guid ticket, QActionPlan param)
        {
            return new SrvResult<PagerResult<MActionPlan2>>()
            {
                Data = ActionPlanBL2.ActionPlanList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> ActionPlanDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = ActionPlanBL2.ActionPlanDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }

        //批量删除
        [Auth]
        [OperationContract]
        public SrvResult<bool> ActionPlanDelForBatch(Guid ticket, int[] rowsID)
        {
            return new SrvResult<bool>()
            {
                Data = ActionPlanBL2.ActionPlanDelForBatch(UserInfo, rowsID),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 获取审核区域
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<MArea[]> AuditAreaGet(Guid ticket, int id = 0)
        {
            return new SrvResult<MArea[]>()
            {
                Data = ActionPlanBL2.AuditAreaGet(UserInfo,id),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 获取员工的审核类型和区域
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<MSetActionPlan> SetActionPlan(Guid ticket, int id)
        {
            return new SrvResult<MSetActionPlan>()
            {
                Data = ActionPlanBL2.SetActionPlan(id),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region LPA审核记录

        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MAction2>> ActionList(Guid ticket, QAction param)
        {
            return new SrvResult<PagerResult<MAction2>>()
            {
                Data = ActionBL.ActionList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> ActionDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = ActionBL.ActionDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<MAction2> LPAActionGet(Guid ticket, int id)
        {
            return new SrvResult<MAction2>()
            {
                Data = ActionBL.Get(id),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<MLPAActionResult[]> LPAActionResultGet(Guid ticket, int id, string area, string keyword)
        {
            return new SrvResult<MLPAActionResult[]>()
            {
                Data = ActionBL.GetResultItems(id, area, keyword),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region 问题管理

        [Auth]
        [OperationContract]
        public SrvResult<MProblem> ProblemGet(Guid ticket, int id)
        {
            return new SrvResult<MProblem>()
            {
                Data = ProblemBL.ProblemGet(id),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<bool> ProblemSave(Guid ticket, MProblem model)
        {
            return new SrvResult<bool>()
            {
                Data = ProblemBL.ProblemSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MProblem>> ProblemList(Guid ticket, QProblem param)
        {
            return new SrvResult<PagerResult<MProblem>>()
            {
                Data = ProblemBL.ProblemList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }


        //看板问题数据
        [WebGet]
        [OperationContract]
        public SrvResult<MProblem[]> KanBanProblemList(int? machineid)
        {
            return new SrvResult<MProblem[]>()
            {
                Data = ProblemBL.KanBanProblemList(machineid),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// add by lidm 2017-05-22：查看问题详情
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [WebGet]
        [OperationContract]
        public SrvResult<MProblem> KanBanProblemDetail(int pid)
        {
            return new SrvResult<MProblem>()
            {
                Data = ProblemBL.ProblemGet(pid),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> ProblemDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = ProblemBL.ProblemDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }

        //导出
        [Auth]
        [OperationContract]
        public SrvResult<string> ProblemExport(Guid ticket, QProblem param)
        {
            return new SrvResult<string>()
            {
                Data = ProblemBL.ProblemExport(UserInfo, param),
                Status = ResultStatus.Success
            };
        }


        [Auth]
        [WebGet]
        [OperationContract]
        public SrvResult<MProblem[]> MineProblemListForApp(Guid ticket)
        {
            return new SrvResult<MProblem[]>()
            {
                Data = ProblemBL.MineProblemList(UserInfo),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [WebGet]
        [OperationContract]
        public SrvResult<MProblem> ProblemInfoForApp(Guid ticket, int id)
        {
            return new SrvResult<MProblem>()
            {
                Data = ProblemBL.ProblemInfo(UserInfo, id),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> ProblemUpdateForApp(Guid ticket, MProblem model)
        {
            return new SrvResult<bool>()
            {
                Data = ProblemBL.ProblemUpdate(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> ProblemFinish(Guid ticket, MProblem model)
        {
            return new SrvResult<bool>()
            {
                Data = ProblemBL.ProblemFinish(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region 终端LPA审核计划查询
        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MActionPlan> GetLPAPlanTobe(Guid ticket)
        {
            return new SrvResult<MActionPlan>()
            {
                Data = ActionPlanBL.GetPlanTobe(UserInfo),
                Status = ResultStatus.Success
            };
        }

        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MActionPlan[]> GetLPAPlanListTobe(Guid ticket)
        {
            return new SrvResult<MActionPlan[]>()
            {
                Data = ActionPlanBL.GetnPlanListTobe(UserInfo),
                Status = ResultStatus.Success
            };
        }

        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MAuditGroup[]> GetLPAAuditItemList(Guid ticket, int planid, string area)
        {
            return new SrvResult<MAuditGroup[]>()
            {
                Data = ActionPlanBL.GetAuditItemList(UserInfo, planid, area),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<int> SaveActionResultList(Guid ticket, int actionid, int state, List<MActionResult> models)
        {
            return new SrvResult<int>()
            {
                Data = ActionPlanBL.SaveActionResultList(actionid, state, models),
                Status = ResultStatus.Success
            };
        }

        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MRegionTypeResponse> GetRegTypRes(Guid ticket, int planid)
        {
            return new SrvResult<MRegionTypeResponse>()
            {
                Data = ActionPlanBL.GetRegionTypeResponseEmp(UserInfo, planid),
                Status = ResultStatus.Success
            };
        }

        [Auth]
        [OperationContract]
        public SrvResult<bool> SubmitProblem(Guid ticket, MProblem problem)
        {
            return new SrvResult<bool>()
            {
                Data = ActionPlanBL.AddProblem(UserInfo, problem),
                Status = ResultStatus.Success
            };
        }


        [Auth]
        [OperationContract]
        public SrvResult<bool> SubmitNcProblem(Guid ticket, MProblem problem)
        {
            return new SrvResult<bool>()
            {
                Data = ActionPlanBL.SubmitNcProblem(UserInfo, problem),
                Status = ResultStatus.Success
            };
        }

        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MProblem[]> GetItemDetail(Guid ticket, int actionid, int itemid)
        {
            return new SrvResult<MProblem[]>()
            {
                Data = ActionPlanBL.GetItemDetail(actionid, itemid),
                Status = ResultStatus.Success
            };
        }

        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<bool> DelProblem(Guid ticket, int probid)
        {
            return new SrvResult<bool>()
            {
                Data = ActionPlanBL.ProblemDel(UserInfo, probid),
                Status = ResultStatus.Success
            };
        }


        [WebGet]
        [Auth]
        [OperationContract]
        public SrvResult<MProblem[]> GetProbList(Guid ticket, int actionid, string area)
        {
            return new SrvResult<MProblem[]>()
            {
                Data = ActionPlanBL.GetProbList(actionid, area),
                Status = ResultStatus.Success
            };
        }
        #endregion


        #region LPA报表

        [OperationContract]
        public SrvResult<MReport[]> ReportQuery(QReport param)
        {
            return new SrvResult<MReport[]>()
            {
                Data = ReportBL.Query(param),
                Status = ResultStatus.Success
            };
        }

        [WebGet]
        [OperationContract]
        public SrvResult<MReport[]> ReportQueryCloseRate()
        {
            return new SrvResult<MReport[]>()
            {
                Data = ReportBL.QueryCloseRate(),
                Status = ResultStatus.Success
            };
        }
        #endregion

        #region 区域维护

        /// <summary>
        /// 查询产线
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MArea>> AreaList(Guid ticket, QArea param)
        {
            return new SrvResult<PagerResult<MArea>>()
            {
                Data = AreaBL.AreaList(UserInfo, param),
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
        public SrvResult<MArea> AreaGet(Guid ticket, int id)
        {
            return new SrvResult<MArea>()
            {
                Data = AreaBL.AreaGet(id),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 新增/编辑区域
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<bool> AreaSave(Guid ticket, MArea model)
        {
            return new SrvResult<bool>()
            {
                Data = AreaBL.AreaSave(UserInfo, model),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<bool> AreaDel(Guid ticket, int id)
        {
            return new SrvResult<bool>()
            {
                Data = AreaBL.AreaDel(UserInfo, id),
                Status = ResultStatus.Success
            };
        }



        #endregion

        #region LPA Overview

        /// <summary>
        /// LPA周视图
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<MLpaOverviewWeek> GetLpaOverviewWeek(QLpaOverviewWeek param)
        {
            return new SrvResult<MLpaOverviewWeek>()
            {
                Data = LpaOverviewBL.GetLpaOverviewWeek(param),
                Status = ResultStatus.Success
            };
        }


        [Auth]
        [OperationContract]
        public SrvResult<MAllWeek[]> GetAllWeek(Guid ticket,int year)
        {
            return new SrvResult<MAllWeek[]>()
            {
                Data = LpaOverviewBL.GetAllWeek(year),
                Status = ResultStatus.Success
            };
        }

        #endregion

        #region LPA各种报表

        /// <summary>
        /// 分层审核问题区域（月）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<MLPAProblemArea[]> LpaProblemArea(QLPAProblemArea param)
        {
            return new SrvResult<MLPAProblemArea[]>()
            {
                Data = LpaReport.LpaProblemArea(param),
                Status = ResultStatus.Success
            };
        }


        /// <summary>
        /// 分层审核问题关闭率(月)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<MLPAProblemArea[]> GetLPAClosedProRateMonthly(QLPAProblemArea param)
        {
            return new SrvResult<MLPAProblemArea[]>()
            {
                Data = LpaReport.GetLPAClosedProRateMonthly(param),
                Status = ResultStatus.Success
            };
        }
        /// <summary>
        /// 分层审核问题分类（月）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<ProCateGroyWkReport> GetLPACategoryMonthly(QLPAProblemArea param)
        {
            return new SrvResult<ProCateGroyWkReport>()
            {
                Data = LpaReport.GetLPACategoryMonthly(param),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 分层审核问题发现人（月）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<ProCateGroyWkReport> GetProFinderMonthReport(QLPAProblemArea param)
        {
            return new SrvResult<ProCateGroyWkReport>()
            {
                Data = LpaReport.GetProFinderMonthReport(param),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 分层审核问题责任人（月）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<ProCateGroyWkReport> GetProRespMonthReport(QLPAProblemArea param)
        {
            return new SrvResult<ProCateGroyWkReport>()
            {
                Data = LpaReport.GetProRespMonthReport(param),
                Status = ResultStatus.Success
            };
        }


        /// <summary>
        /// 分层审核问题区域(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<WeeksReport> GetLPAAreaWeekly(QLPAProblemArea param)
        {
            return new SrvResult<WeeksReport>()
            {
                Data = LpaReport.GetLPAAreaWeekly(param),
                Status = ResultStatus.Success
            };
        }


        /// <summary>
        /// 分层审核问题关闭率(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<WeeksReport> GetLpaCpWeekRate(QLPAProblemArea param)
        {
            return new SrvResult<WeeksReport>()
            {
                Data = LpaReport.GetLpaCpWeekRate(param),
                Status = ResultStatus.Success
            };
        }

        /// <summary>
        /// 分层审核问题分类(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<ProCateGroyWkReport> GetProCategoryWeekRate(QLPAProblemArea param)
        {
            return new SrvResult<ProCateGroyWkReport>()
            {
                Data = LpaReport.GetProCategoryWeekRate(param),
                Status = ResultStatus.Success
            };
        }


        /// <summary>
        /// 分层审核问题发现人(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<ProCateGroyWkReport> GetProFinderWeekReport(QLPAProblemArea param)
        {
            return new SrvResult<ProCateGroyWkReport>()
            {
                Data = LpaReport.GetProFinderWeekReport(param),
                Status = ResultStatus.Success
            };
        }
        /// <summary>
        /// 分层审核问题责任人(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<ProCateGroyWkReport> GetProRespWeekly(QLPAProblemArea param)
        {
            return new SrvResult<ProCateGroyWkReport>()
            {
                Data = LpaReport.GetProRespWeekly(param),
                Status = ResultStatus.Success
            };
        }
        #endregion


        ///<summary>
        ///录入数据的审核项汇总
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Auth]
        [OperationContract]
        public SrvResult<PagerResult<MAuditItem>> AuditSummaryList(Guid ticket, QAction param)
        {
            return new SrvResult<PagerResult<MAuditItem>>()
            {
                Data = ActionBL.AuditSummaryList(UserInfo, param),
                Status = ResultStatus.Success
            };
        }






        /// <summary>
        /// 计划执行完成率报表（月）
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        
        [OperationContract]
        public SrvResult<MLPAProblemArea[]> GetPlanFinishedRate(QLPAProblemArea param)
        {
            return new SrvResult<MLPAProblemArea[]>()
            {
                Data = LpaReport.GetPlanFinishedRate(param),
                Status=ResultStatus.Success


            };

        }

        /// <summary>
        /// 计划执行完成率报表（周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        public SrvResult<WeeksReport> GetPlanFinishedRateWeekly(QLPAProblemArea param)
        {
            return new SrvResult<WeeksReport>()
            {
                Data = LpaReport.GetPlanFinishedRateWeekly(param),
                Status = ResultStatus.Success
            };
        }






        ////邮件计划，问题列表测试
        //[Auth]
        //[OperationContract]
        //public SrvResult<string> GetMailData(Guid ticket)
        //{
        //    return new SrvResult<string>()
        //    {
        //        Data = MailListBL.GetMailData(UserInfo),
        //        Status = ResultStatus.Success
        //    };
        //}
        ////邮件计划，问题列表测试2
        //[Auth]
        //[OperationContract]
        //public SrvResult<bool> CheckExpired(Guid ticket)
        //{
        //    return new SrvResult<bool>()
        //    {
        //        Data = ActionPlanBL.CheckExpired(),
        //        Status = ResultStatus.Success
        //    };
        //}




    }
}
