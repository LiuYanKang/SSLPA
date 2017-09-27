using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Entity;
using EntityFramework.Extensions;
using EntityFramework.Future;

namespace SeekerSoft.Core.DB
{

    public static class DBHelper<T> where T : class
    {
        public static PagerResult<T> GetPage<TOrder>(DbContext db, PagerParams pageParams, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order, bool IsDesc = false)
        {
            pageParams.pageIndex += 1;//由于miniui pageIndex 默认从0开始，所以这边加1
            var query = db.Set<T>().Where(where);
            FutureQuery<T> queryDataSet = null;
            if (string.IsNullOrWhiteSpace(pageParams.sortOrder) || string.IsNullOrWhiteSpace(pageParams.sortOrder))
            {
                if (IsDesc)
                {
                    //倒序
                    queryDataSet = query.OrderByDescending(order).GetPage(pageParams).Future();
                }
                else
                {
                    //正序
                    queryDataSet = query.OrderBy(order).GetPage(pageParams).Future();
                }
            }
            else
            {
                var property = typeof(T).GetProperty(pageParams.sortField);
                var parameter = Expression.Parameter(typeof(T), "sortOrder");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                string methodName = pageParams.sortOrder.ToUpper() == "DESC" ? "OrderByDescending" : "OrderBy";
                MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExp));
                query = query.Provider.CreateQuery<T>(resultExp);
                queryDataSet = query.GetPage(pageParams).Future();
            }

            PagerResult<T> pagerResult = new PagerResult<T>();
            pagerResult.Data = queryDataSet.ToList();
            pagerResult.TotalCount = query.FutureCount().Value;
            pagerResult.PageIndex = pageParams.pageIndex - 1;
            pagerResult.PageSize = pageParams.pageSize;
            return pagerResult;
        }

        public static System.Data.Entity.Infrastructure.DbRawSqlQuery<T> ExecuteSqlToEntities(DbContext db, string sql, params SqlParameter[] parameters)
        {
            return db.Database.SqlQuery<T>(sql, parameters);
        }

        public static PagerResult<T> ExecuteSqlPageProcedureToEntities(DbContext db, SqlPageProcedureParams sqlPageProcedureParams)
        {
            sqlPageProcedureParams.pageIndex += 1;//由于miniui pageIndex 默认从0开始，所以这边加1
            SqlParameter[] SqlParameters = {
					new SqlParameter("@outRecordCount", SqlDbType.BigInt),
					new SqlParameter("@outPageSize", SqlDbType.BigInt),
					new SqlParameter("@outPageIndex", SqlDbType.BigInt),
                    new SqlParameter("@pageSize", SqlDbType.BigInt),
					new SqlParameter("@pageIndex", SqlDbType.BigInt),
					new SqlParameter("@searchField", SqlDbType.VarChar,5000),
					new SqlParameter("@afterFromBeforeOrderBySqlString", SqlDbType.VarChar,8000),
                    new SqlParameter("@orderByField", SqlDbType.VarChar,200)};

            SqlParameters[0].Direction = ParameterDirection.Output;
            SqlParameters[1].Direction = ParameterDirection.Output;
            SqlParameters[2].Direction = ParameterDirection.Output;
            SqlParameters[3].Value = sqlPageProcedureParams.pageSize;
            SqlParameters[4].Value = sqlPageProcedureParams.pageIndex;
            SqlParameters[5].Value = sqlPageProcedureParams.searchField.Trim();
            SqlParameters[6].Value = sqlPageProcedureParams.afterFromBeforeOrderBySql;
            SqlParameters[7].Value = sqlPageProcedureParams.orderByField;

            IEnumerable<T> queryDataSet = db.Database.SqlQuery<T>(@"
                EXEC PageProcedure 
                @outRecordCount out,
                @outPageSize out,
                @outPageIndex out,
                @pageSize,
                @pageIndex,
                @searchField,
                @afterFromBeforeOrderBySqlString,
                @orderByField", SqlParameters);

            PagerResult<T> pagerResult = new PagerResult<T>();
            pagerResult.Data = queryDataSet.ToList();
            pagerResult.TotalCount = string.IsNullOrEmpty(SqlParameters[0].Value.ToString()) ? 0 : (Int64)Convert.ToInt32(SqlParameters[0].Value.ToString());
            pagerResult.PageSize = (Int64)Convert.ToInt32(SqlParameters[1].Value.ToString());
            pagerResult.PageIndex = (Int64)Convert.ToInt32(SqlParameters[2].Value.ToString()) - 1;
            return pagerResult;
        }
    }
}
