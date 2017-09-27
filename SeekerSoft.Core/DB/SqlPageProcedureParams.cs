using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.DB
{
    public class SqlPageProcedureParams
    {
        private Int64 _pageSize = 10;

        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        public Int64 pageSize
        {
            set { _pageSize = value; }
            get { return _pageSize; }
        }

        private Int64 _pageIndex = 0;

        /// <summary>
        /// 当前页数
        /// </summary>
        public Int64 pageIndex
        {
            set { _pageIndex = value; }
            get { return _pageIndex; }
        }

        /// <summary>
        /// SQL语句中所要查询的字段集合 例如 tb1.AA,tb2.BB.tb3.CC
        /// </summary>
        private string _searchField = "*";

        public string searchField
        {
            get { return _searchField; }
            set { _searchField = value; }
        }

        /// <summary>
        /// SQL语句from之后一直到order by之前的内容（包括条件 where） 例如 Table1 tb1 where tb1.AA='1'
        /// </summary>
        private string _afterFromBeforeOrderBySql;

        public string afterFromBeforeOrderBySql
        {
            get { return _afterFromBeforeOrderBySql; }
            set { _afterFromBeforeOrderBySql = value; }
        }

        /// <summary>
        /// order by 的排序字段的集合 例如 tb1.AA desc
        /// </summary>
        private string _orderByField = "ID";

        public string orderByField
        {
            get { return _orderByField; }
            set { _orderByField = value; }
        }

        /// <summary>
        /// sql 字符串
        /// </summary>
        public string sql
        {
            get
            {
                _sql = null;
                _sql += " select ";
                _sql += searchField;
                _sql += " from ";
                _sql += afterFromBeforeOrderBySql;
                _sql += " order by ";
                _sql += orderByField;
                return _sql;
            }
            set
            {
                _sql = value.ToLower();
                int indexFrom = _sql.IndexOf("select") + "select".Length;
                int indexTo = _sql.IndexOf("from");
                searchField = value.Substring(indexFrom, indexTo - indexFrom).Trim();
                indexFrom = indexTo + "from".Length;
                indexTo = _sql.LastIndexOf("order by");
                afterFromBeforeOrderBySql = value.Substring(indexFrom, indexTo - indexFrom).Trim();
                indexFrom = indexTo + "order by".Length;
                orderByField = value.Substring(indexFrom).Trim();
            }
        }
        private string _sql = null;

        public static string GetSortSQL(string sortField, string sortOrder)
        {
            return string.Format(" {0} {1} ", sortField, "DESC".Equals(sortOrder, StringComparison.CurrentCultureIgnoreCase) ? "DESC" : "ASC");
        }

        public static string GetSortSQL(SortParam[] SortParam)
        {
            if (SortParam == null || SortParam.Length == 0) return null;
            StringBuilder sbOrderStr = new StringBuilder();
            foreach (var li in SortParam)
            {
                sbOrderStr.AppendFormat(" {0} {1} ,", li.sortField, "DESC".Equals(li.sortOrder, StringComparison.CurrentCultureIgnoreCase) ? "DESC" : "ASC");
            }
            sbOrderStr.Remove(sbOrderStr.Length - 1, 1);
            return sbOrderStr.ToString();
        }
    }
}
