using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    /// <summary>
    /// 产线实体
    /// </summary>
    [DataContract]
    public class MProductLine
    {
        /// <summary>
        /// 产线ID
        /// </summary>
        [DataMember]
        public int? ProLineId { get; set; }

        /// <summary>
        /// 产品线路代码
        /// </summary>
        [DataMember]
        public string ProLineCode { get; set; }

        /// <summary>
        /// 产品线路名称
        /// </summary>
        [DataMember]
        public string ProLineName { get; set; }

        /// <summary>
        /// 是否删除
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
        public int CreateBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [DataMember]
        public int? ModifyBy { get; set; }

        /// <summary>
        /// 修改人名称
        /// </summary>
        [DataMember]
        public string UpdateName { get; set; }

        /// <summary>
        /// 关联的设备
        /// </summary>
        [DataMember]
        public MMachine[] MachineList { get; set;}

        /// <summary>
        /// 产品代码
        /// </summary>
        [DataMember]
        public string PDCode { get; set; }

        /// <summary>
        /// 产线设备
        /// </summary>
        [DataMember]
        public string MachineNames { get; set; }

    }
}
