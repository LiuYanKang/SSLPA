using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    /// <summary>
    /// 相应审核项目的设备和其他实体
    /// </summary>
    [DataContract]
    public class MAuditMachine
    {
        /// <summary>
        /// 审核设备和其他
        /// </summary>
        [DataMember]
        public string MCode { get; set;}

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
