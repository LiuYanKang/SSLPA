using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MMachineDown
    {
        [DataMember]
        public int? CloseDownID { get; set; }
        [DataMember]
        public int? MachineID { get; set; }
        [DataMember]
        public int? DownCodeID { get; set; }
        [DataMember]
        public string DownCode { get; set; }
        [DataMember]
        public string DownCodeDesc { get; set; }
        [DataMember]
        public string MachineCode { get; set; }
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string MachineStatus { get; set; }
        [DataMember]
        public string MachineStatusName { get; set; }
        [DataMember]
        public string Quality { get; set; }
        [DataMember]
        public string TroubleType { get; set; }
        [DataMember]
        public string TroubleTypeName { get; set; }
        [DataMember]
        public DateTime? SubmitTime { get; set; }
        [DataMember]
        public DateTime? RecoveryTime { get; set; }
        [DataMember]
        public int? DurationTime { get; set; }
        [DataMember]
        public string SubmitPersonName { get; set; }
    }
}
