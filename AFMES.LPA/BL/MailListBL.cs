
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SSLPA.DB;
using SSLPA.LPA.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.BL
{
  public static class MailListBL
    {
       
        //测试：查询今日执行计划与问题数据
        public static string  GetMailData(UserInfo userInfo)
        {
            //没有传入id
            string sql1 = @"SELECT  t1.PlanID ,
        t1.EmpID,
        CONVERT(varchar(100), t1.StartPlanDate, 120) AS StartDate,
        CONVERT(varchar(100), t1.EndPlanDate, 120) AS EndDate,
		EmpName=t2.Name,
        t1.StartPlanDate ,
        t1.EndPlanDate ,
        t1.AuditType ,
		AuditTypeName=t3.Name,
        t1.IsComplete ,
        t1.ActionTime,
        t1.BanCi 
	    FROM lpa.ActionPlan t1
		LEFT JOIN base.Employee t2 ON t2.EmpID = t1.EmpID
		LEFT JOIN base.DicItem t3 ON t1.AuditType=t3.Code AND t3.DicCode='1024'
		WHERE t1.IsDel=0   AND DateDiff(dd, t1.StartPlanDate, getdate())= 0";


            string sql2 = @"
SELECT  t1.ProbID ,
        t1.ActionID ,
        t1.ItemID ,
        t1.ProblemRegion ,
		ProblemRegionName=t2.AreaName,
        t1.ProblemType ,
		ProblemTypeName=t3.Name,
        t1.Responsible ,
		ResponsibleName=t4.Name,
        t1.MachineID ,
		MachineName=t7.Name,
        t1.ProblemDesc ,
        t1.SubmitDate ,
        t1.PlanStartDate ,
        t1.PlanEndDate ,
        t1.ActualEndDate ,
        t1.Measure ,
        t1.State ,
        t1.CreateTime,
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
		LEFT JOIN dic.Machine t7 ON t1.MachineID = t7.MachineID
		WHERE t1.IsDel=0 ";
            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var dataplan = DBHelper<MActionPlan2>.ExecuteSqlToEntities(db, sql1).ToArray();
                //if (dataplan == null)
                //    throw new Exception("找不到指定的数据。");


                var dataproblem = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql2).ToArray();
                

                var list = new MMailList();
                list.DataPlan = dataplan;
                list.DataProblem = dataproblem;

                string data = GetMailList(list,"LPAAAA");

                return data;

               
            }

         

        }

        //传入查询数据，拼接页面
        public static string GetMailList(MMailList data, string title,bool isShowPlan=true,bool isShowProblem=true)
            
        {
            string mailBody;

            string url = SeekerSoft.Core.Config.ConfigHelper.Get("MailLogin");


            mailBody = "<div> <table border"+0+" cellpadding="+0+" cellspacing=\"0\" width=\"100% \" style=\"font-size: 13px; color:#333; line-height:20px \"><tbody><tr><td>";
            mailBody += "<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"900\" style=\"border: none; border-collapse: collapse; \">";
            mailBody += "<tbody><tr><td style=\"padding: 10px 0; border: none; vertical-align: middle; \"><strong style=\"font-size: 16px\">"+title+"</strong></td></tr></tbody></table> ";
            mailBody += " <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"900\" style=\"border-collapse: collapse; background-color: #fff; border: 1px solid #cfcfcf; box-shadow: 0 0px 6px rgba(0, 0, 0, 0.1); margin-bottom: 20px; font-size:13px;\">";


            mailBody += "<tbody>";
            //日期与网址链接
            mailBody += "<tr><td>";
            mailBody += "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; border: none; border-collapse: collapse; \">";
            mailBody += "<tbody><tr>";
            //日期
            mailBody += "<td style=\"padding: 10px; background-color: #F8FAFE; border: none; font-size: 14px; font-weight: 500; border-bottom: 1px solid #e5e5e5;\">"+ DateTime.Now +"</td>";
            mailBody += "<td style=\"width: 40px; text-align: right; background-color: #F8FAFE; border: none; vertical-align: top; padding: 10px; border-bottom: 1px solid #e5e5e5;\">";
            //网址
           // mailBody += "<a href=\"http://lpa.seeksoft.com/\" target=\"_blank\">http://lpa.seeksoft.com/</a>";

            mailBody += "<a href ="+ url+" target=\"_blank\">"+url+"</a>";

            mailBody += "</td></tr></tbody></table></td></tr>";


            if (isShowPlan == true)
            {

                //1.0 今日执行计划
                mailBody += "<tr><td style=\"padding: 10px; border: none; \"><h5 style=\"margin: 8px 0; font-size: 14px; \">今日执行计划,共有" + data.DataPlan.Length + "个</h5>";
                mailBody += "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; border: 1px solid #e5e5e5; margin-bottom: 15px; border-collapse: collapse; font-size: 13px;\">";
                mailBody += "<tbody><tr>";
                mailBody += "<th style=\"width:100px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">开始时间</th>";
                mailBody += "<th style=\"width:100px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">结束时间</th>";
                mailBody += "<th style=\"width:80px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">执行人</th>";
                mailBody += "<th style=\"width:80px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">审核类型</th>";
                mailBody += "<th style=\"width:80px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">区域</th>";
                mailBody += "<th style=\"border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">班次</th>";
                mailBody += " </tr>";

                //计划列表
                for (int j = 0; j < data.DataPlan.Length; j++)
                {
                    mailBody += "<tr><td style=\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataPlan[j].StartDate + "</td>";
                    mailBody += "<td style=\"padding: 5px; border: 1px solid #e5e5e5;\">";
                    mailBody += "<a href=\""+url+" target=\"_blank\">" + data.DataPlan[j].EndDate + "</a></td>";
                    mailBody += "<td style =\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataPlan[j].Name + "</td>";
                    mailBody += "<td style =\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataPlan[j].AuditTypeName + "</td>";
                    mailBody += "<td style =\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataPlan[j].AudtiAreaMes + "</td>";
                    mailBody += "<td style =\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataPlan[j].BanCiName + "</td>";

                    mailBody += "</tr>";
                }
                mailBody += "</tbody></table></td></tr>";

            }

            if (isShowProblem == true)
            {

                //2.0 问题列表
                mailBody += "<tr><td style=\"padding: 10px; border: none; \"><h5 style=\"margin: 8px 0; font-size: 14px; \">尚未完成的问题,共有" + data.DataProblem.Length + "个</h5>";
                mailBody += "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 100%; border: 1px solid #e5e5e5; margin-bottom: 15px; border-collapse: collapse; font-size: 13px;\">";
                mailBody += "<tbody><tr>";
                mailBody += "<th style=\"width: 50px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">序号</th>";
                mailBody += "<th style=\"width: 70px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">发现人</th>";
                mailBody += "<th style=\"width: 70px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">责任人</th>";
                mailBody += "<th style=\"width: 80px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">创建时间</th>";
                mailBody += "<th style=\"width: 80px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">计划完成时间</th>";
                mailBody += "<th style=\"width: 50px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">区域</th>";
                mailBody += "<th style=\"width: 50px; border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">分类</th>";
               
                mailBody += "<th style=\"border: 1px solid #e5e5e5; background-color: #f5f5f5; padding: 5px;\">问题描述</th>";
                mailBody += " </tr>";

                //问题列表
                for (int j = 0; j < data.DataProblem.Length; j++)
                {
                    int i = j + 1;
                    mailBody += "<tr><td style=\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + i + "</td>";
                    mailBody += "<td style=\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataProblem[j].CreateByName + "</td>";
                    mailBody += "<td style=\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataProblem[j].Name + "</td>";
                    mailBody += "<td style=\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataProblem[j].StartDate + "</td>";
                    mailBody += "<td style=\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataProblem[j].EndDate + "</td>";
                    mailBody += "<td style=\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataProblem[j].ProblemRegionName + "</td>";
                    mailBody += "<td style=\"padding: 5px; text-align: center; border: 1px solid #e5e5e5;\">" + data.DataProblem[j].ProblemTypeName + "</td>";

                    


                    mailBody += "<td style=\"padding: 5px; border: 1px solid #e5e5e5;\">";
                    mailBody += "<a href=\""+url+" target=\"_blank\">" + data.DataProblem[j].ProblemDesc + "</a></td>";
                    mailBody += "</tr>";
                }
                mailBody += "</tbody></table></td></tr>";

            }


            mailBody += "</tbody></table></td></tr></tbody></table></div> ";
            return mailBody;


         }
    }
}
