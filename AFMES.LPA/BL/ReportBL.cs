using SSLPA.DB;
using SSLPA.LPA.DTO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.BL
{
    public class ReportBL
    {
        public static MReport[] Query(QReport param)
        {
            string sql = null;
            switch (param.ByType)
            {
                case "finder":// 发现人
                    sql = @"SELECT  t2.EmpID ,
        t2.Name ,
        ProbCount= COUNT(t1.ProbID)
FROM    lpa.Problem t1
        INNER JOIN base.Employee t2 ON t2.EmpID = t1.CreateBy
WHERE   t1.IsDel = 0
    AND t1.CreateTime BETWEEN @begindate AND @enddate
GROUP BY t2.EmpID ,
        t2.Name
ORDER BY COUNT(t1.ProbID) DESC";
                    break;
                case "region":// 区域
                    sql = @"SELECT  
        t2.AreaName ,
        ProbCount= COUNT(t1.ProbID)
FROM    lpa.Problem t1
        INNER JOIN lpa.Area t2 ON t2.AreaId = t1.ProblemRegion 
WHERE   t1.IsDel = 0
    AND t1.CreateTime BETWEEN @begindate AND @enddate
GROUP BY t2.AreaCode ,
        t2.AreaName
ORDER BY COUNT(t1.ProbID) DESC";
                    break;
                case "resp":// 责任人
                    sql = @"SELECT  t2.EmpID ,
        t2.Name ,
        ProbCount= COUNT(t1.ProbID)
FROM    lpa.Problem t1
        INNER JOIN base.Employee t2 ON t2.EmpID = t1.Responsible
WHERE   t1.IsDel = 0
    AND t1.CreateTime BETWEEN @begindate AND @enddate
GROUP BY t2.EmpID ,
        t2.Name
ORDER BY COUNT(t1.ProbID) DESC";
                    break;
                case "date":// 发现日期
                    sql = @"
SELECT  
        Name = CONVERT(NVARCHAR(10),t1.CreateTime ,120),
        ProbCount= COUNT(t1.ProbID)
FROM    lpa.Problem t1
WHERE   t1.IsDel = 0
AND t1.CreateTime BETWEEN @begindate AND @enddate
GROUP BY CONVERT(NVARCHAR(10),t1.CreateTime ,120)
ORDER BY COUNT(t1.ProbID) DESC ";
                    break;
                default:
                    throw new Exception("报表类型参数异常");
            }


            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MReport>.ExecuteSqlToEntities(db, sql,
                    new System.Data.SqlClient.SqlParameter("begindate", param.BeginDate),
                    new System.Data.SqlClient.SqlParameter("enddate", param.EndDate)).ToArray();
                return data;
            }
        }


        public static MReport[] QueryCloseRate()
        {
            string sql = @"SELECT  t1.Responsible ,
        Name = t4.Name ,
        ProbCount = SUM(CASE WHEN t1.Progress >= 100 THEN 1
                 ELSE 0
            END) * 100 / COUNT(1)
FROM    lpa.Problem t1
        LEFT JOIN base.Employee t4 ON t1.Responsible = t4.EmpID
WHERE   t1.IsDel = 0
GROUP BY t1.Responsible ,
        t4.Name
ORDER BY ProbCount DESC";

            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MReport>.ExecuteSqlToEntities(db, sql).ToArray();
                return data;
            }
        }
    }
}
