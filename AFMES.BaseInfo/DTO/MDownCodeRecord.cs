using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MDownCodeRecord
    {
        [DataMember]
        public int CloseDownID { get; set; }
        [DataMember]
        public int MachineID { get; set; }     
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public string EquipStatus { get; set; }
        [DataMember]
        public string EquipStatusName { get; set; }
        [DataMember]
        public int CloseDownCode { get; set; }
        [DataMember]
        public string CloseDownCodeName { get; set; }
        [DataMember]
        public string TroubleType { get; set; }
        [DataMember]
        public string TroubleTypeName { get; set; }
        [DataMember]
        public DateTime? SubmitTime { get; set; }
        [DataMember]
        public int SubmitPerson { get; set; }
        [DataMember]
        public string SubmitPersonName { get; set; }
        [DataMember]
        public int? DurationTime { get; set; }
    }
}
