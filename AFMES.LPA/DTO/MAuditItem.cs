using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MAuditItem
    {
        [DataMember]
        public int? ItemID { get; set; }
        [DataMember]
        public string AuditType { get; set; }
        [DataMember]
        public string ItemRegion { get; set; }
        [DataMember]
        public string ItemType { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int ActionID { get; set; }
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public string ItemRegionName { get; set; }
        [DataMember]
        public string ItemTypeName { get; set; }
        [DataMember]
        public Nullable<int> Result { get; set; }
        [DataMember]
        public int SN { get; set; }
        [DataMember]
        public string EnterNum { get; set; }
        [DataMember]
        public string MCode { get; set; }
        [DataMember]
        public string MName { get; set; }
        [DataMember]
        public string PDCode { get; set; }

        [DataMember]
        public bool IsInputData { get; set; }
    }
}
