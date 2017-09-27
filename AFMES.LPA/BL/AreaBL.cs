using SSLPA.DB;
using SSLPA.LPA.DTO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.BL
{
    public class AreaBL
    {
        #region 区域维护

        /// <summary>
        /// 查询区域
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static PagerResult<MArea> AreaList(UserInfo userInfo, QArea param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"lpa.Area AS t1 WITH ( NOLOCK )
                          LEFT JOIN base.Employee t2 ON t1.ModifyBy = t2.UserID
                          WHERE  1 = 1 AND t1.IsDel = 0  ");
            // 非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t1.PDCode = '{0}' ", pdCode);
                }
            }
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat("  AND ( t1.AreaId LIKE '%{0}%' OR t1.AreaCode LIKE '%{0}%'  OR t1.AreaName LIKE '%{0}%' OR t1.PDCode LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
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
            pageProcedureParams.searchField = @"t1.AreaId ,t1.AreaCode ,t1.AreaName ,t1.UpdateTime ,UpdateName = t2.Name,t1.CreateTime,t1.PDCode,
                       MachineNames= STUFF((SELECT ','+mc.Name FROM lpa.AreaMachineMap  AS  amm WITH(NOLOCK)
LEFT JOIN dic.Machine AS mc ON amm.MachineID=mc.MachineID
WHERE amm.AreaId=t1.AreaId   FOR XML PATH('')),1,1,'')   ";
            pageProcedureParams.afterFromBeforeOrderBySql = sql.ToString();
            pageProcedureParams.orderByField = SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder);
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MArea>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }


        /// <summary>
        /// 查询某一个区域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MArea AreaGet(int id)
        {
            string sql = @"SELECT t1.AreaId ,t1.AreaCode ,t1.AreaName ,t1.UpdateTime ,t1.CreateTime,t1.PDCode
                          FROM   lpa.Area AS t1 WITH ( NOLOCK ) WHERE t1.AreaId=@id";
            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MArea>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");
                sql = @"SELECT t1.MachineID,t1.Name,t1.Code,t1.ProductType,WorkProcessName=t2.Name FROM lpa.AreaMachineMap  AS  t3 WITH(NOLOCK)
LEFT JOIN dic.Machine AS t1 ON t3.MachineID=t1.MachineID
LEFT JOIN dic.WorkProcess AS  t2 ON t1.ProcID=t2.ProcID
WHERE t3.AreaId=@areaId  ";
                model.MachineList = DBHelper<BaseInfo.DTO.MMachine>.ExecuteSqlToEntities(db, sql, new SqlParameter("@areaId", id)).ToArray();
                return model;
            }
        }

        /// <summary>
        /// 新增/编辑产品部门
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool AreaSave(UserInfo userinfo, MArea model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Area.SingleOrDefault(_ => _.AreaId == model.AreaId);
                //如果数据库中存在这条数据，则进行编辑操作
                if (entity != null)
                {
                    entity.AreaCode  = model.AreaCode;
                    entity.IsDel = false;
                    entity.AreaName  = model.AreaName;
                    entity.PDCode = model.PDCode;
                    entity.UpdateTime = DateTime.Now;
                    entity.ModifyBy = userinfo.UserID;
                }
                else
                {
                    //新增
                    entity = new Area()
                    {
                        AreaCode  = model.AreaCode,
                        AreaName = model.AreaName,
                        IsDel = false,
                        PDCode = model.PDCode,
                        CreateBy = userinfo.UserID,
                        CreateTime = DateTime.Now
                    };
                    db.Area.Add(entity);
                    db.SaveChanges();
                    model.AreaId = entity.AreaId;
                }

                // 移除所有已有权限
                db.AreaMachineMap.RemoveRange(db.AreaMachineMap.Where(_ => _.AreaId ==model.AreaId));
                if (model.MachineList != null)
                {
                    for (var i = 0; i < model.MachineList.Length; i++)
                    {
                        db.AreaMachineMap.Add(new AreaMachineMap()
                        {
                            MachineID = model.MachineList[i].MachineID.Value,
                            AreaId  = model.AreaId.Value
                        });
                    }
                }

                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool AreaDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.Area.FirstOrDefault(_ => _.AreaId  == id);
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        
        #endregion
    }
}
