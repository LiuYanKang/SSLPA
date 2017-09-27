using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    /// <summary>
    /// 设备信息
    /// </summary>
    [DataContract]
    public class MMachine
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        [DataMember]
        public int? MachineID { get; set; }
        /// <summary>
        /// 所属工序ID
        /// </summary>
        [DataMember]
        public int ProcID { get; set; }
        /// <summary>
        /// 停机代码ID
        /// </summary>
        [DataMember]
        public Nullable<int> DownCodeID { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 设备代码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        [DataMember]
        public string ProductType { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        [DataMember]
        public string ProductTypeName { get; set; }
        /// <summary>
        /// 设备处理速度
        /// </summary>
        [DataMember]
        public double Speed { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        [DataMember]
        public string Status { get; set; }
        /// <summary>
        /// 设备实时状态
        /// </summary>
        [DataMember]
        public string NowStatus { get; set; }
        /// <summary>
        /// QS状态
        /// </summary>
        [DataMember]
        public string Quality { get; set; }
        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public string ProcCode { get; set; }
        [DataMember]
        public string WorkProcessName { get; set; }
        [DataMember]
        public string StatusName { get; set; }
        [DataMember]
        public string NowStatusName { get; set; }
        [DataMember]
        public string QualityName { get; set; }

        [DataMember]
        public string QualityWarning { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int SN { get; set; }
    }
}
