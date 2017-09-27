using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    /// <summary>
    /// 区域实体
    /// </summary>
    [DataContract]
    public class MArea
    {
        /// <summary>
        /// 产线ID
        /// </summary>
        [DataMember]
        public int? AreaId { get; set; }

        /// <summary>
        /// 产品线路代码
        /// </summary>
        [DataMember]
        public string AreaCode { get; set; }

        /// <summary>
        /// 产品线路名称
        /// </summary>
        [DataMember]
        public string AreaName { get; set; }

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
        public MMachine[] MachineList { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        [DataMember]
        public string PDCode { get; set; }
    }
}
