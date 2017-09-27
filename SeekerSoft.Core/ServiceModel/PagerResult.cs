using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Core.ServiceModel
{
    public class PagerResult<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Data { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public long PageIndex { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        public long PageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// 获得总页数，根据总记录数和每页行数计算
        /// </summary>
        /// <returns></returns>
        public long GetTotalPages()
        {
            return (TotalCount - 1) / PageSize + 1;
        }

        /// <summary>
        /// 汇总
        /// </summary>
        [DataMember]
        public object[] Summary { get; set; }
    }
}
