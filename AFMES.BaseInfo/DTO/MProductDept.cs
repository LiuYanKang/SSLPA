using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    /// <summary>
    /// 产品部门实体
    /// </summary>
    [DataContract]
    public class MProductDept
    {
        /// <summary>
        /// 产品代码
        /// </summary>
        [DataMember]
        public string PDCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 是否删除(0:删除，1：未删除)
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public int? CreateBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 修改人ID 
        /// </summary>
        [DataMember]
        public int? ModifyBy { get; set; }

        /// <summary>
        ///更新人
        /// </summary>
        [DataMember]
        public string UpdateName { get; set; }
    }
}
