using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    /// <summary>
    /// 产线设备映射表
    /// </summary>
    [DataContract]
    public class MLineMachineMap
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        [DataMember]
        public int? MachineID { get; set; }

        /// <summary>
        /// 产线ID
        /// </summary>
        [DataMember]
        public int? ProLineId { get; set; }

        /// <summary>
        /// 产品部门代码
        /// </summary>
        [DataMember]
        public int? PDCode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int? SN { get; set; }
    }
}
