using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Core.ServiceModel
{
    [DataContract]
    public class PagerParams
    {
        /// <summary>
        /// 每页行数
        /// </summary>
        [DataMember]
        public int pageSize { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        [DataMember]
        public int pageIndex { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        [DataMember]
        public string sortField { get; set; }

        /// <summary>
        /// 顺序类型
        /// </summary>
        [DataMember]
        public string sortOrder { get; set; }

        [DataMember]
        public SortParam[] SortParam { get; set; }

        public int Skip
        {
            get { return (pageIndex - 1) * pageSize; }
        }
    }

    /// <summary>
    /// 排序
    /// </summary>
    [DataContract]
    public class SortParam
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="order"></param>
        public SortParam(string field, bool orderAsc = true)
        {
            sortField = field;
            sortOrder = orderAsc ? "ASC" : "DESC";
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        [DataMember]
        public string sortField { get; set; }

        /// <summary>
        /// 顺序类型
        /// </summary>
        [DataMember]
        public string sortOrder { get; set; }
    }
}
