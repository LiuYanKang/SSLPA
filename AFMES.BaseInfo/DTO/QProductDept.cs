using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    /// <summary>
    /// 产品部门查询条件字段
    /// </summary>
    public class QProductDept : PagerParams
    {
        /// <summary>
        /// 产品部门代码
        /// </summary>
        public string Keyword { get; set; }
    }
}
