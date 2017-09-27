using SSLPA.BaseInfo.DTO;
using SSLPA.DB;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.BL
{
    public class MachineBL
    {
        public static PagerResult<MMachine> MachineQuery(UserInfo userInfo, QMachine pagerParams)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"
SELECT  t1.MachineID ,
        t1.Name ,
        WorkProcessName = t2.Name ,
        t1.DownCodeID ,
        t1.Speed ,
        t1.ProductType ,
        ProductTypeName = t5.Name ,
        t1.Status ,
        StatusName = t3.Name ,
        t1.NowStatus ,
        NowStatusName = nt3.Name ,
        t1.Quality ,
        QualityName = t4.Name ,
        t1.Code ,
        t1.Remark,
        t1.QualityWarning
FROM    dic.Machine t1
        LEFT JOIN dic.WorkProcess t2 ON t2.ProcID = t1.ProcID
        LEFT JOIN base.DicItem t3 ON t1.Status = t3.Code AND t3.DicCode = '1004'
        LEFT JOIN base.DicItem nt3 ON t1.NowStatus = nt3.Code AND nt3.DicCode = '1004'
        LEFT JOIN base.DicItem t4 ON t1.Quality = t4.Code AND t4.DicCode = '1005'
        LEFT JOIN dic.ProductDept t5 ON t1.ProductType = t5.PDCode
WHERE   t1.IsDel = 0 ");

            // 非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t5.PDCode = '{0}' ", pdCode);
                }
            }

            if (!string.IsNullOrEmpty(pagerParams.Keyword))
            {
                sql.AppendFormat(" AND (t1.Code LIKE '%{0}%' OR t1.Name LIKE '%{0}%' OR t2.Name LIKE '%{0}%')", pagerParams.Keyword.Replace("'", "''"));
            }
            if (!string.IsNullOrEmpty(pagerParams.ProductType))
            {
                sql.AppendFormat(" AND t1.ProductType = '{0}'", pagerParams.ProductType);
            }

            if (pagerParams.ProcID.HasValue)
            {
                sql.AppendFormat(" AND (t2.ProcID = '{0}')", pagerParams.ProcID);
            }
            // 排序
            if (string.IsNullOrEmpty(pagerParams.sortField)) pagerParams.sortField = "t1.Code";

            switch (pagerParams.sortField)
            {
                case "ProductTypeName":
                    pagerParams.sortField = "t1.ProductType"; break;
                case "Code":
                    pagerParams.sortField = "t1.Code"; break;
                case "Name":
                    pagerParams.sortField = "t1.Name"; break;
                case "ProcID":
                    pagerParams.sortField = "t1.ProcID"; break;
                case "Remark":
                    pagerParams.sortField = "t1.Remark"; break;
                case "StatusName":
                    pagerParams.sortField = "t3.Name"; break;
                case "NowStatusName":
                    pagerParams.sortField = "nt3.Name"; break;
            }
            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(pagerParams.sortField, pagerParams.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = pagerParams.pageSize;
            pageProcedureParams.pageIndex = pagerParams.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                PagerResult<MMachine> data = DBHelper<MMachine>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        public static MMachine[] QueryAll(int id)
        {
            var sql = string.Format(@"SELECT  t1.MachineID ,
        t1.Code,
        t1.Name  FROM dic.Machine AS t1 WITH(NOLOCK)
		LEFT JOIN lpa.AreaMachineMap AS t2 ON t1.MachineID=t2.MachineID
		WHERE IsDel=0 AND t2.AreaId={0} ORDER BY Code ASC",id);
            using (var db = DBHelper.NewDB())
            {
                return db.Database.SqlQuery<MMachine>(sql).ToArray();
            }
        }

        public static MMachine MachineGet(UserInfo userinfo, int id)
        {
            string sql = @"SELECT  t1.MachineID ,
        t1.Name,
        WorkProcessName = t2.Name ,
        t2.ProcID,
        t1.Code ,
        t1.ProductType ,
        t1.Speed,
        t1.Remark,
        t1.QualityWarning
        FROM    dic.Machine t1
        LEFT JOIN dic.WorkProcess t2 ON t2.ProcID = t1.ProcID
	    WHERE t1.MachineID = " + id;
            using (var db = DB.DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var data = DBHelper<MMachine>.ExecuteSqlToEntities(db, sql).SingleOrDefault();
                if (data == null)
                    throw new Exception("找不到指定的数据。");
                return data;
            }
        }

        public static MMachine GetByCode(string code)
        {
            string sql = @"SELECT  t1.MachineID ,
        t1.Name,
        WorkProcessName = t2.Name ,
        t2.ProcID,
        t2.Code AS ProcCode,
        t1.Code ,
        t1.Speed,
        t1.Remark
        FROM    dic.Machine t1
        LEFT JOIN dic.WorkProcess t2 ON t2.ProcID = t1.ProcID
	    WHERE t1.IsDel=0 AND t1.Code = @code ";
            using (var db = DB.DBHelper.NewDB())
            {
                return DBHelper<MMachine>.ExecuteSqlToEntities(db, sql, new SqlParameter("code", code)).SingleOrDefault();
            }
        }

        public static bool MachineAdd(UserInfo userinfo, MMachine model)
        {
            using (var db = DBHelper.NewDB())
            {
                if (db.Machine.Any(_ => !_.IsDel && _.Code == model.Code))
                    throw new Exception("该设备已存在!");

                //新增
                db.Machine.Add(new Machine()
                {
                    Name = model.Name,
                    Status = "2",
                    Quality = "1",
                    ProcID = model.ProcID,
                    Speed = model.Speed,
                    Code = model.Code,
                    ProductType = model.ProductType,
                    Remark = model.Remark,
                    IsDel = false,
                    CreateBy = userinfo.UserID,
                    CreateTime = DateTime.Now,
                });
                db.SaveChanges();
            }
            return true;
        }

        public static bool MachineSave(UserInfo userinfo, MMachine model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Machine.Where(_ => _.MachineID == model.MachineID).SingleOrDefault();
                if (entity == null) throw new Exception("找不到指定的数据");
                if (db.Machine.Any(_ => !_.IsDel && _.Code == model.Code && _.Code != entity.Code))
                    throw new Exception("该设备已存在!");
                entity.ProcID = model.ProcID;
                entity.Name = model.Name;
                entity.Speed = model.Speed;
                entity.Code = model.Code;
                entity.ProductType = model.ProductType;
                entity.Remark = model.Remark;
                entity.ModifyBy = userinfo.UserID;
                entity.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        public static bool MachineQualityWarningSave(UserInfo userinfo, MMachine model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Machine.SingleOrDefault(_ => _.MachineID == model.MachineID);
                if (entity == null) throw new Exception("找不到指定的数据");
                entity.QualityWarning = model.QualityWarning;
                entity.UpdateTime = DateTime.Now;
                entity.ModifyBy = userinfo.UserID;
                db.SaveChanges();
            }
            return true;
        }

        public static bool MachineDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.Machine.Where(_ => _.MachineID == id).FirstOrDefault();
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        public static void GetInfo(int id)
        {

        }

        /// <summary>
        /// 获取默认拉拔设备
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static Machine GetDrawMachine(AFMESEntities db)
        {
            var machine = db.Machine.Where(_ => _.ProcID == 1 && !_.IsDel).FirstOrDefault();
            return machine;
        }

        /// <summary>
        /// 获取半成品生产设备状态信息
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public static MMachineState GetMachineStateSemi(UserInfo userinfo, string machineCode)
        {
            string sql = @"SELECT  t2.ExecutionID ,
        t1.MachineID ,
		t1.Name ,
		t1.Speed ,
		t1.ProductType ,
		t1.Status ,
		t1.Quality ,
        t2.OrderID ,
		t3.No ,
		t3.SemiID ,
		t4.Code AS SemiCode ,
		t4.Diameter ,
        t2.OrderType ,
		t2.StartTime ,
        t2.ExpectTime ,
        t2.FinishTime ,
        t2.ExeStatus 
		FROM dic.Machine t1
		LEFT JOIN prod.Execution t2 ON t2.MachineID = t1.MachineID AND t2.ExeStatus = '1'
		LEFT JOIN prod.SemiProductOrder t3 ON t3.SemiOrderID = t2.OrderID AND t2.OrderType = '1' AND t3.IsDel = 0
		LEFT JOIN dic.SemiProduct t4 ON t4.SemiProdID = t3.SemiID AND t4.IsDel = 0
	    WHERE t1.Code = @code AND t1.IsDel = 0";
            using (var db = DB.DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var data = DBHelper<MMachineState>.ExecuteSqlToEntities(db, sql, new SqlParameter("code", machineCode)).SingleOrDefault();
                if (data == null)
                    throw new Exception("找不到指定的数据。");
                return data;
            }
        }
        /// <summary>
        /// 获取产品生产设备状态信息
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="machineCode"></param>
        /// <returns></returns>
        public static MMachineState GetMachineStateProd(UserInfo userinfo, string machineCode)
        {
            string sql = @"SELECT  t2.ExecutionID ,
        t1.MachineID ,
		t1.Name ,
		t1.Status ,
		t1.Quality ,
        t2.OrderID ,
		t3.No ,
		t3.ProdID ,
		t3.Quantity ,
		t4.Code AS ProdCode ,
        t2.OrderType ,
		t2.StartTime ,
        t2.ExeStatus ,
		t4.CustCode ,
		t5.Logo ,
		ProducedQuantity = t6.Quantity
		FROM dic.Machine t1
		LEFT JOIN prod.Execution t2 ON t2.MachineID = t1.MachineID AND t2.ExeStatus = '1'
		LEFT JOIN prod.ProductOrder t3 ON t3.ProdOrderID = t2.OrderID AND t2.OrderType = '2' AND t3.IsDel = 0
		LEFT JOIN dic.Product t4 ON t4.ProdID = t3.ProdID AND t4.IsDel = 0
		LEFT JOIN dic.Customer t5 ON t5.CustID = t4.CustID AND t5.IsDel = 0
		LEFT JOIN (SELECT tOU.ExecutionID,Quantity=SUM(tOU.Quantity) FROM prod.OutputUpdate tOU GROUP BY tOU.ExecutionID) t6 ON t2.ExecutionID = t6.ExecutionID
	    WHERE t1.Code = @code AND t1.IsDel = 0";
            using (var db = DB.DBHelper.NewDB())
            {
                var data = DBHelper<MMachineState>.ExecuteSqlToEntities(db, sql, new SqlParameter("code", machineCode)).SingleOrDefault();
                if (data == null)
                    throw new Exception("找不到指定的数据。");
                // QS倒计时
                string qsLastTime = @"SELECT  ISNULL(t7.FinishTime ,t3.StartTime)
FROM dic.Machine t1
	INNER JOIN dic.WorkProcess t2 ON t2.ProcID=t1.ProcID AND t2.ProcType = '3'
	INNER JOIN prod.Execution t3 ON  t3.MachineID = t1.MachineID AND t3.ExeStatus = '1'
	LEFT JOIN (
		SELECT qs1.MachineID,qs1.FinishTime
		FROM qs.QuantityCheck qs1
		INNER JOIN (
			SELECT  MachineID,BeginTime=MAX(BeginTime)
			FROM qs.QuantityCheck
			GROUP BY MachineID ) qs2 ON qs2.MachineID = qs1.MachineID AND qs2.BeginTime = qs1.BeginTime
	) t7 ON t7.MachineID = t1.MachineID
WHERE t1.MachineID=@machineID";
                data.LastQSTime = db.Database.SqlQuery<DateTime?>(qsLastTime, new SqlParameter("machineID", data.MachineID)).SingleOrDefault();

                return data;
            }
        }


        public static MMachineDown GetMachineDownInfo(UserInfo userinfo, string machineCode)
        {
            string sql = @"
SELECT TOP 1
        t2.CloseDownID ,
		t1.MachineID ,
        DownCodeID = t2.CloseDownCode ,
        t3.Code AS DownCode ,
        t1.Code AS MachineCode ,
        t1.Name AS MachineName ,
        Status = t2.EquipStatus ,
        t1.Quality ,
        t2.TroubleType ,
        t4.Name AS TroubleTypeName ,
        t2.SubmitTime
FROM    dic.Machine t1
        LEFT JOIN down.CloseDownDetail t2 ON t2.MachineID = t1.MachineID
        LEFT JOIN dic.DownCode t3 ON t3.DownCodeID = t2.CloseDownCode
        LEFT JOIN base.DicItem t4 ON t4.Code = t2.TroubleType
                                     AND t4.DicCode = '1019'
WHERE   t1.IsDel = 0
        AND t1.Code = @machineCode
ORDER BY t2.CloseDownID DESC";
            using (var db = DB.DBHelper.NewDB())
            {
                var data = DBHelper<MMachineDown>.ExecuteSqlToEntities(db, sql, new SqlParameter("machineCode", machineCode)).FirstOrDefault();
                if (data == null) throw new Exception("找不到指定的数据。");
                return data;
            }
        }

        public static PagerResult<MMachineDown> QueryCloseDownByEWI(int machineid, PagerParams param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"
SELECT  t1.CloseDownID ,
        MachineStatus = t1.EquipStatus ,
        MachineStatusName = t6.Name ,
        DownCode = t3.Code ,
        DownCodeDesc = t3.Summary ,
        TroubleTypeName = t4.Name ,
        t1.SubmitTime ,
		t1.RecoveryTime ,
		DurationTime = ISNULL(t1.DurationTime, DATEDIFF(MINUTE,t1.SubmitTime,GETDATE()) ),
        SubmitPersonName = ISNULL(t5.Name, '系统')
FROM    down.CloseDownDetail t1
        LEFT JOIN dic.DownCode t3 ON t3.DownCodeID = t1.CloseDownCode
        LEFT JOIN base.DicItem t4 ON t4.Code = t1.TroubleType
                                     AND t4.DicCode = '1019'
        LEFT JOIN base.Employee t5 ON t5.UserID = t1.SubmitPerson
        LEFT JOIN base.DicItem t6 ON t6.Code = t1.EquipStatus
                                     AND t6.DicCode = '1004'
WHERE   (t1.DurationTime IS NULL OR t1.DurationTime >5)
");
            if (machineid > 0)
                sql.AppendFormat(" AND t1.MachineID = {0}", machineid);

            // 排序
            if (string.IsNullOrEmpty(param.sortField))
            {
                param.sortField = "t1.CloseDownID";
                param.sortOrder = "DESC";
            }
            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DB.DBHelper.NewDB())
            {
                var data = DBHelper<MMachineDown>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        /// <summary>
        /// 根据产线ID获取设备
        /// </summary>
        /// <param name="productLineId"></param>
        /// <returns></returns>
        public static MMachine[] GetLineMachineList(int productLineId)
        {
            var sql = string.Format(@"SELECT  t1.MachineID ,t1.Code,
        t1.Name,WorkProcessName=t3.Name  FROM dic.Machine AS t1 WITH(NOLOCK)
		LEFT JOIN dic.LineMachineMap AS t2 ON t1.MachineID=t2.MachineID
		LEFT JOIN dic.WorkProcess t3 ON t3.ProcID = t1.ProcID
		WHERE t1.IsDel=0 AND t2.ProLineId={0} ORDER BY t1.Code ASC", productLineId);
            using (var db = DBHelper.NewDB())
            {
                return db.Database.SqlQuery<MMachine>(sql).ToArray();
            }
        }

        /// <summary>
        /// 根据工序ID获取设备
        /// </summary>
        /// <param name="procId"></param>
        /// <returns></returns>
        public static MMachine[] GetWorkMachineList(int procId)
        {
            var sql = string.Format(@"SELECT  t1.MachineID ,t1.Code,
        t1.Name,WorkProcessName=t3.Name  FROM dic.Machine AS t1 WITH(NOLOCK)
		LEFT JOIN dic.WorkProcess t3 ON t3.ProcID = t1.ProcID
		WHERE t1.IsDel=0 AND t3.ProcID={0} ORDER BY t1.Code ASC", procId);
            using (var db = DBHelper.NewDB())
            {
                return db.Database.SqlQuery<MMachine>(sql).ToArray();
            }
        }

    }
}