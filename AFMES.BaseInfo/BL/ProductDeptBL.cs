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
    public class ProductDeptBL
    {

        #region 产品部门维护

        /// <summary>
        /// 查询产品部门列表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static PagerResult<MProductDept> ProductDeptList(UserInfo userInfo, QProductDept param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@" SELECT t1.PDCode ,t1.Name ,t1.CreateTime ,t1.UpdateTime ,UpdateName = t2.Name
                          FROM   dic.ProductDept AS t1 WITH ( NOLOCK )
                          LEFT JOIN base.Employee t2 ON t1.ModifyBy = t2.UserID
                          WHERE  1 = 1 AND t1.IsDel = 0 ");
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat("  AND ( t1.PDCode LIKE '%{0}%' OR t1.Name LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
            }
            // 排序
            if (string.IsNullOrEmpty(param.sortField))
            {
                param.sortField = " t1.CreateTime "; param.sortOrder = "DESC";
            }
            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MProductDept>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        /// <summary>
        /// 查询某一个产品部门
        /// </summary>
        /// <param name="pdcode"></param>
        /// <returns></returns>
        public static MProductDept ProductDeptGet(string pdcode)
        {
            string sql = @"SELECT t1.PDCode,t1.Name,t1.IsDel,t1.CreateTime,t1.CreateBy,t1.UpdateTime,t1.ModifyBy 
                           FROM dic.ProductDept AS t1 WITH(NOLOCK) 
                           WHERE 1=1 AND t1.PDCode=@pdcode";
            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MProductDept>.ExecuteSqlToEntities(db, sql, new SqlParameter("@pdcode", pdcode)).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");
                return model;
            }
        }

        /// <summary>
        /// 新增/编辑产品部门
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool ProductDeptSave(UserInfo userinfo, MProductDept model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.ProductDept.SingleOrDefault(_ => _.PDCode == model.PDCode);
                //如果数据库中存在这条数据，则进行编辑操作
                if (entity != null)
                {
                    entity.Name = model.Name;
                    entity.IsDel =false;
                    entity.UpdateTime = DateTime.Now;
                    entity.ModifyBy = userinfo.UserID;
                }
                else
                {
                    //新增
                    entity = new ProductDept()
                    {
                        PDCode = model.PDCode,
                        Name = model.Name,
                        IsDel = false,
                        CreateBy = userinfo.UserID,
                        CreateTime = DateTime.Now
                    };
                    db.ProductDept.Add(entity);
                }
                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 删除产品部门
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="pdcode"></param>
        /// <returns></returns>
        public static bool ProductDeptDel(UserInfo userinfo, string pdcode)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.ProductDept.FirstOrDefault(_ => _.PDCode == pdcode);
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
