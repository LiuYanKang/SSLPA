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
    public class ProductLineBL
    {
        #region 产线维护

        /// <summary>
        /// 查询产线
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static PagerResult<MProductLine> ProductLineList(UserInfo userInfo, QProductLine param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"dic.ProductLine AS t1 WITH ( NOLOCK )
                          LEFT JOIN base.Employee t2 ON t1.ModifyBy = t2.UserID
                          WHERE  1 = 1 AND t1.IsDel = 0  ");

            // 非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t2.PDCode = '{0}' ", pdCode);
                }
            }
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat("  AND ( t1.ProLineId LIKE '%{0}%' OR t1.ProLineCode LIKE '%{0}%'  OR t1.ProLineName LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
            }
            // 排序
            if (string.IsNullOrEmpty(param.sortField))
            {
                param.sortField = " t1.CreateTime "; param.sortOrder = "DESC";
            }
            //sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.searchField = @"t1.ProLineId ,t1.ProLineCode ,t1.ProLineName ,t1.UpdateTime ,UpdateName = t2.Name,t1.CreateTime,
                          MachineNames=STUFF((SELECT ','+mc.Name FROM dic.LineMachineMap  AS  lmm WITH(NOLOCK)
LEFT JOIN dic.Machine AS mc ON lmm.MachineID=mc.MachineID
WHERE lmm.ProLineId=t1.ProLineId ORDER BY lmm.SN ASC  FOR XML PATH('')),1,1,'')    ";
            pageProcedureParams.afterFromBeforeOrderBySql = sql.ToString();
            pageProcedureParams.orderByField = SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder);
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MProductLine>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        /// <summary>
        /// 查询某一个产线
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MProductLine ProductLineGet(int id)
        {
            string sql = @"SELECT t1.ProLineId ,t1.ProLineCode ,t1.ProLineName ,t1.UpdateTime ,t1.CreateTime,t1.PDCode
                          FROM   dic.ProductLine AS t1 WITH ( NOLOCK ) WHERE t1.ProLineId=@id";
            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MProductLine>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");
                sql = @"SELECT t1.MachineID,t1.Name,t1.Code,t1.ProductType,WorkProcessName=t2.Name,t3.SN FROM dic.LineMachineMap  AS  t3 WITH(NOLOCK)
LEFT JOIN dic.Machine AS t1 ON t3.MachineID=t1.MachineID
LEFT JOIN dic.WorkProcess AS  t2 ON t1.ProcID=t2.ProcID
WHERE t3.ProLineId=@proLineId ORDER BY t3.SN ASC ";
                model.MachineList = DBHelper<MMachine>.ExecuteSqlToEntities(db, sql, new SqlParameter("@proLineId", id)).ToArray();
                return model;
            }
        }

        public static MProductDept[] ProDept()
        {
            using (var db = DBHelper.NewDB())
            {
                return db.Database.SqlQuery<MProductDept>(@"
 SELECT t1.PDCode ,
        t1.Name ,
        t1.CreateTime ,
        t1.UpdateTime ,
        UpdateName = t2.Name
 FROM   dic.ProductDept AS t1 WITH ( NOLOCK )
 LEFT JOIN base.Employee t2 ON t1.ModifyBy = t2.UserID
 WHERE  1 = 1
        AND t1.IsDel = 0 ORDER BY t1.PDCode ASC
").ToArray();
            }
        }

        /// <summary>
        /// 新增/编辑产品部门
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool ProductLineSave(UserInfo userinfo, MProductLine model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.ProductLine.SingleOrDefault(_ => _.ProLineId == model.ProLineId);
                //如果数据库中存在这条数据，则进行编辑操作
                if (entity != null)
                {
                    entity.ProLineCode = model.ProLineCode;
                    entity.IsDel = false;
                    entity.ProLineName = model.ProLineName;
                    entity.PDCode = model.PDCode;
                    entity.UpdateTime = DateTime.Now;
                    entity.ModifyBy = userinfo.UserID;
                }
                else
                {
                    //新增
                    entity = new ProductLine()
                    {
                        ProLineCode = model.ProLineCode,
                        ProLineName = model.ProLineName,
                        IsDel = false,
                        PDCode = model.PDCode,
                        CreateBy = userinfo.UserID,
                        CreateTime = DateTime.Now
                    };
                    db.ProductLine.Add(entity);
                }

                // 移除所有已有权限
                db.LineMachineMap.RemoveRange(db.LineMachineMap.Where(_ => _.ProLineId == model.ProLineId));
                if (model.MachineList != null)
                {
                    for (var i = 0; i < model.MachineList.Length; i++)
                    {
                        db.LineMachineMap.Add(new LineMachineMap()
                        {
                            MachineID = Convert.ToInt32(model.MachineList[i].MachineID),
                            ProLineId = Convert.ToInt32(model.ProLineId),
                            PDCode = model.PDCode,
                            SN = i + 1
                        });
                    }
                }

                db.SaveChanges();
            }
            return true;
        }


        /// <summary>
        /// 拖拽之后 保存审核项目数据
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static bool ProductLineSnSave(UserInfo userinfo, int[] idList)
        {
            using (var db = DBHelper.NewDB())
            {
                for (int i = 0; i < idList.Length; i++)
                {
                    int itemid = idList[i];
                    var entity = db.LineMachineMap.Where(_ => _.MachineID == itemid).SingleOrDefault();
                    if (entity != null)
                    {
                        entity.SN = i;
                    }
                }

                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 删除产线
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool ProductLineDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.ProductLine.FirstOrDefault(_ => _.ProLineId == id);
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 获取某一产品部门下的产线
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="param"></param>
        /// <param name="pDCode"></param>
        /// <returns></returns>
        public static PagerResult<MProductLine> GetProductLineList(UserInfo userInfo, QProductLine param,string pDCode)
        {
            StringBuilder sqlWhere = new StringBuilder();
            sqlWhere.AppendFormat(@"SELECT t1.ProLineId ,t1.ProLineCode ,t1.ProLineName ,t1.UpdateTime ,t1.CreateTime,t1.PDCode
                          FROM   dic.ProductLine AS t1 WITH ( NOLOCK ) WHERE t1.IsDel=0 AND t1.PDCode='{0}'",pDCode);
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sqlWhere.AppendFormat("  AND (t1.ProLineName LIKE '%{0}%'  OR t1.ProLineCode LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
            }
            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = "ProLineId";

            sqlWhere.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));
            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sqlWhere.ToString();
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MProductLine>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        #endregion
    }
}
