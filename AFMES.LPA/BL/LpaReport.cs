using SSLPA.DB;
using SSLPA.LPA.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeekerSoft.Core.DB;
using System.Globalization;
using SeekerSoft.Base.BL;

namespace SSLPA.LPA.BL
{

    public class LpaReport
    {
        

        /// <summary>
        /// 分层审核问题区域（月）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static MLPAProblemArea[] LpaProblemArea(QLPAProblemArea param)
        {
            string sql = @"SELECT AreaMonth=MONTH(t2.CreateTime), ProblemCount=COUNT(t2.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE MONTH(t3.CreateTime)=MONTH(t2.CreateTime) AND t3.Progress=100 AND t3.State='2') AS FLOAT)
FROM lpa.Area AS t1
LEFT JOIN lpa.Problem AS t2 ON t1.AreaId=t2.ProblemRegion
WHERE YEAR(t2.CreateTime) = @byYear   AND t1.AreaId=@areaId      
GROUP BY MONTH(t2.CreateTime) 
ORDER BY MONTH(t2.CreateTime) ";
            using (var db = DBHelper.NewDB())
            {
                if (param.AreaId == 0)
                    throw new Exception("必须提供工序筛选条件");
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear),
                   new System.Data.SqlClient.SqlParameter("areaId", param.AreaId)).ToArray();
                return data;
            }
        }

        /// <summary>
        /// 分层审核问题关闭率(月)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static MLPAProblemArea[] GetLPAClosedProRateMonthly(QLPAProblemArea param)
        {
            
            string sql = @"SELECT AreaMonth=MONTH(t2.CreateTime), ProblemCount=COUNT(t2.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE MONTH(t3.CreateTime)=MONTH(t2.CreateTime) AND t3.Progress=100 AND t3.State='2') AS FLOAT)

,CloseRate=Round(CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE MONTH(t3.CreateTime)=MONTH(t2.CreateTime) AND t3.Progress=100 AND t3.State='2') AS FLOAT)/COUNT(t2.ProbID),2)*100

FROM lpa.Area AS t1
LEFT JOIN lpa.Problem AS t2 ON t1.AreaId=t2.ProblemRegion
WHERE YEAR(t2.CreateTime) = @byYear      
GROUP BY MONTH(t2.CreateTime) 
ORDER BY MONTH(t2.CreateTime) ";

            using (var db = DBHelper.NewDB())
            {
               
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
                return data;
            }


           
        }

        /// <summary>
        /// 分层审核问题分类（月）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ProCateGroyWkReport GetLPACategoryMonthly(QLPAProblemArea param)
        {
           
            string sql = @"SELECT AreaMonth=MONTH(t1.CreateTime),  
ProblemCount=COUNT(t1.ProbID) ,
CloseCount=CAST(1.0*(select COUNT(t3.ProbID) from lpa.Problem as t3
where MONTH(t3.CreateTime)=MONTH(t1.CreateTime) AND t3.ProblemType=t1.ProblemType AND t3.Progress=100 AND t3.State='2') AS FLOAT)
,t2.Name,t1.ProblemType,
ProblemCountSum=CAST(1.0*(select COUNT(t3.ProbID) from lpa.Problem as t3
where MONTH(t3.CreateTime)=MONTH(t1.CreateTime) and  t3.ProblemType!='') AS FLOAT),
CloseCountSum=CAST(1.0*(select COUNT(t3.ProbID) from lpa.Problem as t3
where MONTH(t3.CreateTime)=MONTH(t1.CreateTime) AND t3.ProblemType!='' AND t3.Progress=100 AND t3.State='2') AS FLOAT)
FROM base.DicItem AS t2
LEFT JOIN lpa.Problem AS t1 ON t1.ProblemType=t2.Code 
WHERE 1=1 AND t2.DicCode='1026' AND YEAR(t1.CreateTime) =@byYear
GROUP BY MONTH(t1.CreateTime),t2.Name,t1.ProblemType

";
 

            using (var db = DBHelper.NewDB())
            {
                var procCategroy = DicBL.DicItems("1026");
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
               
                var resout = new ProCateGroyWkReport();
                resout.Data = data;
               
                
                resout.ProCategroyList = procCategroy.Select(_ => _.Name).OrderBy(_ => _).ToArray();
                return resout;
            }

        }

        /// <summary>
        /// 分层审核问题发现人（月）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ProCateGroyWkReport GetProFinderMonthReport(QLPAProblemArea param)
        {

            string sql = @"SELECT AreaMonth=MONTH(t1.CreateTime), ProblemCount=COUNT(t1.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE MONTH(t3.CreateTime)=MONTH(t1.CreateTime) AND t3.CreateBy=t1.CreateBy AND t3.Progress=100 AND t3.State='2') AS FLOAT)
,t2.Name
FROM lpa.Problem AS t1
LEFT JOIN base.Employee AS t2 ON t1.CreateBy=t2.EmpID
WHERE 1=1 AND YEAR(t1.CreateTime) = @byYear  
GROUP BY MONTH(t1.CreateTime),t2.Name,t1.CreateBy ";
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
                var resout = new ProCateGroyWkReport();
                resout.Data = data;
                resout.NameList = data.Select(_ => _.Name).OrderBy(_ => _).Distinct().ToArray();
                return resout;
            }

        }

        /// <summary>
        /// 分层审核问题责任人（月）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ProCateGroyWkReport GetProRespMonthReport(QLPAProblemArea param)
        {

            string sql = @"SELECT AreaMonth=MONTH(t1.CreateTime), ProblemCount=COUNT(t1.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE MONTH(t3.CreateTime)=MONTH(t1.CreateTime) AND t3.Responsible=t1.Responsible AND t3.Progress=100 AND t3.State='2') AS FLOAT)
,t2.Name
FROM lpa.Problem AS t1
LEFT JOIN base.Employee AS t2 ON t1.Responsible=t2.EmpID
WHERE 1=1 AND YEAR(t1.CreateTime) = @byYear  
GROUP BY MONTH(t1.CreateTime),t2.Name,t1.Responsible ";
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
                var resout = new ProCateGroyWkReport();
                resout.Data = data;
                resout.NameList = data.Select(_ => _.Name).OrderBy(_ => _).Distinct().ToArray();
                return resout;
            }

        }







        /// <summary>
        /// 分层审核问题区域（周）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static WeeksReport GetLPAAreaWeekly(QLPAProblemArea param)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int lastWeek = gc.GetWeekOfYear(new DateTime(Convert.ToInt32(param.ByYear), 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);//当前年的最后一周
            string[] week = new string[lastWeek];
            for (var i = 0; i < lastWeek; i++)
            {
                var wk = i + 1;
                week[i] = wk.ToString();
            }
            string sql = @"SELECT Weeks=datepart(wk,t2.CreateTime), ProblemCount=COUNT(t2.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE datepart(wk,t3.CreateTime)=datepart(wk,t2.CreateTime) AND t3.Progress=100 AND t3.State='2') AS FLOAT)
FROM lpa.Area AS t1
LEFT JOIN lpa.Problem AS t2 ON t1.AreaId=t2.ProblemRegion
WHERE YEAR(t2.CreateTime) = @byYear   AND t1.AreaId=@areaId      
GROUP BY datepart(wk,t2.CreateTime) 
ORDER BY datepart(wk,t2.CreateTime)
";
            using(var db = DBHelper.NewDB())
            {
                if (param.AreaId == 0)
                    throw new Exception("必须提供工序筛选条件");
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear),
                   new System.Data.SqlClient.SqlParameter("areaId", param.AreaId)).ToArray();

                var resout = new WeeksReport();
                resout.Data = data;
                resout.Weeks = week;
                return resout;
            }
            



        }


        /// <summary>
        /// 分层审核问题关闭率(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static WeeksReport GetLpaCpWeekRate(QLPAProblemArea param)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int lastWeek = gc.GetWeekOfYear(new DateTime(Convert.ToInt32(param.ByYear), 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);//当前年的最后一周
            string[] week = new string[lastWeek];
            for (var i = 0; i < lastWeek; i++)
            {
                var wk = i + 1;
                week[i] =  wk.ToString();
            }
            string sql = @"SELECT Weeks=datepart(wk,t1.CreateTime), ProblemCount=COUNT(t1.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE datepart(wk,t3.CreateTime)=datepart(wk,t1.CreateTime) AND t3.Progress=100 AND t3.State='2') AS FLOAT)
,CloseRate=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE datepart(wk,t3.CreateTime)=datepart(wk,t1.CreateTime) AND t3.Progress=100 AND t3.State='2') AS FLOAT)/COUNT(t1.ProbID)*100
FROM lpa.Problem AS t1
WHERE 1=1 AND YEAR(t1.CreateTime) = @byYear 
GROUP BY datepart(wk,t1.CreateTime) ";
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
                var resout = new WeeksReport();
                resout.Data = data;
                resout.Weeks = week;
                return resout;
            }
        }


        




        /// <summary>
        /// 分层审核问题分类(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ProCateGroyWkReport GetProCategoryWeekRate(QLPAProblemArea param)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int lastWeek = gc.GetWeekOfYear(new DateTime(Convert.ToInt32(param.ByYear), 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);//当前年的最后一周
            string[] week = new string[lastWeek];
            for (var i = 0; i < lastWeek; i++)
            {
                var wk = i + 1;
                week[i] = wk.ToString();
            }
            string sql = @"SELECT Weeks=datepart(wk,t1.CreateTime), ProblemCount=COUNT(t1.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE datepart(wk,t3.CreateTime)=datepart(wk,t1.CreateTime) AND t3.ProblemType=t1.ProblemType AND t3.Progress=100 AND t3.State='2') AS FLOAT)
,t2.Name
FROM base.DicItem AS t2
LEFT JOIN lpa.Problem AS t1 ON t1.ProblemType=t2.Code 
WHERE 1=1 AND t2.DicCode='1026' AND YEAR(t1.CreateTime) = @byYear  
GROUP BY datepart(wk,t1.CreateTime),t2.Name,t1.ProblemType ";
            using (var db = DBHelper.NewDB())
            {
                var procCategroy = DicBL.DicItems("1026");
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
                var resout = new ProCateGroyWkReport();
                resout.Data = data;
                resout.Weeks = week;
                resout.ProCategroyList= procCategroy.Select(_ => _.Name).OrderBy(_ => _).ToArray();
                return resout;
            }
        }


        /// <summary>
        /// 分层审核问题发现人(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ProCateGroyWkReport GetProFinderWeekReport(QLPAProblemArea param)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int lastWeek = gc.GetWeekOfYear(new DateTime(Convert.ToInt32(param.ByYear), 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);//当前年的最后一周
            string[] week = new string[lastWeek];
            for (var i = 0; i < lastWeek; i++)
            {
                var wk = i + 1;
                week[i] = wk.ToString();
            }
            string sql = @"SELECT Weeks=datepart(wk,t1.CreateTime), ProblemCount=COUNT(t1.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE datepart(wk,t3.CreateTime)=datepart(wk,t1.CreateTime) AND t3.CreateBy=t1.CreateBy AND t3.Progress=100 AND t3.State='2') AS FLOAT)
,t2.Name
FROM lpa.Problem AS t1
LEFT JOIN base.Employee AS t2 ON t1.CreateBy=t2.EmpID
WHERE 1=1 AND YEAR(t1.CreateTime) = @byYear 
GROUP BY datepart(wk,t1.CreateTime),t2.Name,t1.CreateBy ";
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
                var resout = new ProCateGroyWkReport();
                resout.Data = data;
                resout.Weeks = week;
                resout.NameList = data.Select(_ => _.Name).OrderBy(_ => _).Distinct().ToArray();
                return resout;
            }
        }



        /// <summary>
        /// 分层审核问题负责人(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static ProCateGroyWkReport GetProRespWeekly(QLPAProblemArea param)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int lastWeek = gc.GetWeekOfYear(new DateTime(Convert.ToInt32(param.ByYear), 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);//当前年的最后一周
            string[] week = new string[lastWeek];
            for (var i = 0; i < lastWeek; i++)
            {
                var wk = i + 1;
                week[i] = wk.ToString();
            }
            string sql = @"
SELECT 

Weeks=datepart(wk,t1.CreateTime), 
ProblemCount=COUNT(t1.ProbID) ,
CloseCount=CAST(1.0*(SELECT COUNT(t3.ProbID) FROM lpa.Problem AS t3 WHERE datepart(wk,t3.CreateTime)=datepart(wk,t1.CreateTime) And t3.Responsible=t1.Responsible and t3.Progress=100 AND t3.State='2') AS FLOAT)
,t2.Name
FROM lpa.Problem AS t1
LEFT JOIN base.Employee AS t2 ON t1.Responsible=t2.EmpID
WHERE 1=1 AND YEAR(t1.CreateTime) = @byYear
GROUP BY datepart(wk,t1.CreateTime),t2.Name,t1.Responsible
 ";
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
                var resout = new ProCateGroyWkReport();
                resout.Data = data;
                resout.Weeks = week;
                resout.NameList = data.Select(_ => _.Name).OrderBy(_ => _).Distinct().ToArray();
                return resout;
            }
        }



        /// <summary>
        /// 计划执行完成率(月)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>

        public static MLPAProblemArea[] GetPlanFinishedRate(QLPAProblemArea param)
        {
            string sql = @"select 
AreaMonth=MONTH(t1.CreateTime),
PlanCountAll=Count(t1.ActionID),
PlanCountFinished=Cast(1.0*(select count(t2.ActionID) from lpa.Action as t2 where Month(t2.CreateTime)=Month(t1.CreateTime) And  t2.State='2') AS FLOAT),
PlanFinishedRate=Round(Cast(1.0*(select count(t2.ActionID) from lpa.Action as t2 where Month(t2.CreateTime)=Month(t1.CreateTime) And  t2.State='2') AS FLOAT)/Count(t1.ActionID)*100,2)
from lpa.Action as t1
WHERE t1.IsDel=0 AND YEAR(t1.CreateTime) = @byYear
group by MONTH(t1.CreateTime)";

            using(var db= DBHelper.NewDB())
            {
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql, new System.Data.SqlClient.SqlParameter("byYear",param.ByYear)).ToArray();
                return data;
            }
            

        }

        /// <summary>
        /// 计划执行完成率(周)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static WeeksReport GetPlanFinishedRateWeekly(QLPAProblemArea param)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int lastWeek = gc.GetWeekOfYear(new DateTime(Convert.ToInt32(param.ByYear), 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);//当前年的最后一周
            string[] week = new string[lastWeek];
            for (var i = 0; i < lastWeek; i++)
            {
                var wk = i + 1;
                week[i] = wk.ToString();
            }
            string sql = @"select 
Weeks=datepart(wk,t1.CreateTime),
PlanCountAll=Count(t1.ActionID),
PlanCountFinished=Cast(1.0*(select count(t2.ActionID) from lpa.Action as t2 where datepart(wk,t2.CreateTime)=datepart(wk,t1.CreateTime) And  t2.State='2') AS FLOAT),
PlanFinishedRate=Round(Cast(1.0*(select count(t2.ActionID) from lpa.Action as t2 where datepart(wk,t2.CreateTime)=datepart(wk,t1.CreateTime) And  t2.State='2') AS FLOAT)/Count(t1.ActionID)*100,2)
from lpa.Action as t1
WHERE t1.IsDel=0 AND YEAR(t1.CreateTime) = @byYear
group by datepart(wk,t1.CreateTime) ";
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MLPAProblemArea>.ExecuteSqlToEntities(db, sql,
                   new System.Data.SqlClient.SqlParameter("byYear", param.ByYear)).ToArray();
                var resout = new WeeksReport();
                resout.Data = data;
                resout.Weeks = week;
                return resout;
            }
        }














    }
}
