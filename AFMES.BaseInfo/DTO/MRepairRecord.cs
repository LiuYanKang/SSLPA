using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    public class MRepairRecord
    {
        [DataMember]
        public int TroubleID { get; set; }
        [DataMember]
        public string CloseDownCodeName { get; set; }
        [DataMember]
        public int MachineID { get; set; }
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public int CloseDownID { get; set; }
        [DataMember]
        public string TroubleType { get; set; }
        [DataMember]
        public string TroubleTypeName { get; set; }
        [DataMember]
        public DateTime CloseDownTime { get; set; }
        [DataMember]
        public string TroubleCode { get; set; }
        [DataMember]
        public string Memo { get; set; }                            
        [DataMember]
        public string RepairResult { get; set; }
        [DataMember]
        public DateTime RepairStartTime { get; set; }
        [DataMember]
        public DateTime? RepairFinishTime { get; set; }
        [DataMember]
        public int? RepairPerson { get; set; }
        [DataMember]
        public string RepairPersonName { get; set; }
        [DataMember]
        public string SubmitPersonName { get; set; }
        [DataMember]
        public int SubmitPerson { get; set; }
        [DataMember]
        public int? ConfirmPerson { get; set; }
        [DataMember]
        public string ConfirmPersonName { get; set; }
    }
}
