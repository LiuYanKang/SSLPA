using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.WXCorp.Model
{
    public class WXOrg
    {
        /// <summary>
        /// 组织架构部门ID
        /// </summary>
        public int? id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 父级部门ID
        /// </summary>
        public int parentid { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string order { get; set; }
    }
}
