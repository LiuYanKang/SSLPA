using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    /// <summary>
    /// 产线查询条件字段
    /// </summary>
    public class QProductLine : PagerParams
    {
        /// <summary>
        /// 产品部门代码
        /// </summary>
        public string Keyword { get; set; }
    }
}
