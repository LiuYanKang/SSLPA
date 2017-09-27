using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MMachineState
    {
        [DataMember]
        public int? ExecutionID { get; set; }
        [DataMember]
        public int MachineID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Speed { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Quality { get; set; }
        [DataMember]
        public int? OrderID { get; set; }
        [DataMember]
        public string No { get; set; }
        [DataMember]
        public int? SemiID { get; set; }
        [DataMember]
        public string SemiCode { get; set; }
        [DataMember]
        public double? Diameter { get; set; }
        [DataMember]
        public string OrderType { get; set; }
        [DataMember]
        public Nullable<System.DateTime> StartTime { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ExpectTime { get; set; }
        [DataMember]
        public Nullable<System.DateTime> FinishTime { get; set; }
        [DataMember]
        public string ExeStatus { get; set; }


        [DataMember]
        public int? ProdID { get; set; }
        [DataMember]
        public int? Quantity { get; set; }
        [DataMember]
        public int? ProducedQuantity { get; set; }
        [DataMember]
        public string ProdCode { get; set; }
        [DataMember]
        public string CustCode { get; set; }
        [DataMember]
        public string Logo { get; set; }

        /// <summary>
        /// 最后QS的时间（用于计算QS倒计时）
        /// </summary>
        [DataMember]
        public DateTime? LastQSTime { get; set; }
    }
}
