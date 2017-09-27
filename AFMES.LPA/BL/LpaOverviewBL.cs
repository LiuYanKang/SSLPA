using SSLPA.LPA.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeekerSoft.Core.DB;
using SSLPA.DB;

namespace SSLPA.LPA.BL
{
    public class LpaOverviewBL
    {
        /// <summary>
        /// LPA周视图
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static MLpaOverviewWeek GetLpaOverviewWeek(QLpaOverviewWeek param)
        {
            MLpaOverviewWeek overviewInfo = new MLpaOverviewWeek();
            //现获取传的 年 和 周 获取日期范围
            if (param.ByWeek < 1)
            {
                throw new Exception("查询的周不对");
            }
            int allDays = (param.ByWeek) * 7;
            //确定当年第一天
            DateTime firstDate = new DateTime(param.ByYear, 1, 1);
            int firstDayOfWeek = (int)firstDate.DayOfWeek;

            firstDayOfWeek = firstDayOfWeek == 0 ? 7 : firstDayOfWeek;

            //周开始日
            int startAddDays = allDays + (1 - firstDayOfWeek);
            DateTime weekRangeStart = firstDate.AddDays(startAddDays);
            //周结束日
            int endAddDays = allDays + (7 - firstDayOfWeek);
            DateTime weekRangeEnd = firstDate.AddDays(endAddDays);

            //if (weekRangeStart.Year > param.ByYear ||
            // (weekRangeStart.Year == param.ByYear && weekRangeEnd.Year > param.ByYear))
            //{
            //    throw new Exception("今年没有第" + param.ByWeek + "周。");
            //}
            string[] strArr = new String[7];
            for (var i = 0; i < strArr.Length; i++)
            {
                strArr[i] = weekRangeStart.AddDays(i).ToString("MM-dd");
            }
            overviewInfo.WeekList = strArr;
            string sql = null;
            sql = @"SELECT t1.PlanID,t3.EmpID,t1.StartPlanDate,t1.EndPlanDate,WeekState=(CASE 
                          WHEN (t2.State IS NULL AND t1.EndPlanDate>=CONVERT(varchar(100), GETDATE(), 23) AND t1.AuditType='1') THEN '5' 
                          WHEN (t1.EndPlanDate<=CONVERT(varchar(100), GETDATE(), 23) AND (t2.State IS NULL OR t2.State=1) AND t1.AuditType='1') THEN '3' 
                          WHEN (t2.State IS NULL AND t1.AuditType!='1' AND t1.EndPlanDate>=CONVERT(varchar(100), GETDATE(), 23) AND t1.StartPlanDate<CONVERT(varchar(100), GETDATE(), 23)) THEN '1'
                          WHEN (t2.State IS NULL AND t1.AuditType!='1' AND t1.EndPlanDate<CONVERT(varchar(100), GETDATE(), 23)) THEN '3'
						  WHEN (t2.State IS NULL AND t1.AuditType!='1' AND t1.StartPlanDate>CONVERT(varchar(100), GETDATE(), 23)) THEN '5'
                          ELSE t2.State  END),t2.State
                           ,t3.Section,Auditor=t4.Name,Period=t5.Name,t1.AuditType FROM lpa.Employee AS t3
LEFT JOIN lpa.ActionPlan AS  t1 ON t1.EmpID=t3.EmpID AND year(t1.StartPlanDate)=@byYear AND datepart(wk,DATEADD(day,-1,t1.StartPlanDate))=@byWeek AND t1.IsDel=0
LEFT JOIN lpa.Action AS  t2 ON t2.PlanID = t1.PlanID
                    LEFT JOIN base.Employee t4 ON t3.EmpID=t4.EmpID
                    LEFT JOIN base.DicItem t5 ON t1.AuditType=t5.Code AND t5.DicCode='1024'
WHERE t3.IsDel=0 ";
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MLpaSection>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear),
                   new System.Data.SqlClient.SqlParameter("byWeek", param.ByWeek)
                   ).ToArray();
                SectionList[] sectionList = data.GroupBy(x => new {x.Section}).Select(k =>
                {
                    var sec = new SectionList() {SectionName = k.Key.Section};
                    EachPeopleList[] eachList= data.Where(j => j.Section == k.Key.Section).GroupBy(y => new {y.Auditor,y.Period}).Select(s =>
                    {
                        EachPeopleList eachP = new EachPeopleList() { Auditor=s.Key.Auditor,Period=s.Key.Period??"" };
                        var peopleState = data.Where(z => z.Auditor == s.Key.Auditor).ToArray();
                            string[] state = new String[7];                                                                                     
                                for (var i = 0; i < strArr.Length; i++)
                                {
                            if (peopleState.Length != 0)
                            {
                                for (var w = 0; w < peopleState.Length; w++)
                                {
                                    if (peopleState[w].AuditType == "1")
                                    {
                                        if (peopleState[w].StartPlanDate.Value.ToString("MM-dd") == strArr[i])
                                        {
                                            state[i] = peopleState[w].WeekState;
                                            break;
                                        }
                                        else
                                        {
                                            state[i] = "4";
                                        }
                                    }
                                    else
                                    {
                                        state[i] = peopleState[w].WeekState;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                state[i] = "4";
                            }
                        }
                                eachP.PeopleWeekState = state;
                            
                                               
                        return eachP;
                    }).ToArray ();
                    sec.EachPeopleList = eachList;
                    return sec;


                }).ToArray ();

                overviewInfo.SectionList = sectionList;
            }
            return overviewInfo;
        }


        public static MAllWeek[] GetAllWeek(int year)
        {
            var allWeek = new List<MAllWeek>();
            var countWeek = 0;
            DateTime tempDate = new DateTime(year, 12, 31);
            int tempDayOfWeek = (int)tempDate.DayOfWeek;
            if (tempDayOfWeek != 0)
            {
                tempDate = tempDate.Date.AddDays(-tempDayOfWeek);
            }
            countWeek=GetWeekIndex(tempDate);
            for (var i = 1; i <= countWeek; i++)
            {
                allWeek.Add(new MAllWeek ()
                {
                    Name =Convert.ToString(i),
                    Value= Convert.ToString(i)
                });
            }

            return allWeek.ToArray();
        }

        /// <summary>
        /// 获取当前年的最大周数 
        /// </summary>
        /// <param name="dTime"></param>
        /// <returns></returns>
        public static int GetWeekIndex(DateTime dTime)
        {
            //需要判断的时间
            //DateTime dTime = Convert.ToDateTime(strDate);
            //确定此时间在一年中的位置
            int dayOfYear = dTime.DayOfYear;

            //DateTime tempDate = new DateTime(dTime.Year,1,6,calendar);
            //当年第一天
            DateTime tempDate = new DateTime(dTime.Year, 1, 1);

            //确定当年第一天
            int tempDayOfWeek = (int)tempDate.DayOfWeek;
            tempDayOfWeek = tempDayOfWeek == 0 ? 7 : tempDayOfWeek;
            //确定星期几
            int index = (int)dTime.DayOfWeek;

            index = index == 0 ? 7 : index;

            //当前周的范围
            DateTime retStartDay = dTime.AddDays(-(index - 1));
            DateTime retEndDay = dTime.AddDays(7 - index);

            //确定当前是第几周
            int weekIndex = (int)Math.Ceiling(((double)dayOfYear + tempDayOfWeek - 1) / 7);


            if (retStartDay.Year < retEndDay.Year)
            {
                weekIndex = 1;
            }

            return weekIndex;
        }

    }
}
