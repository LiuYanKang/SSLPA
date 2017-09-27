using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MAction
    {
        [DataMember]
        public int ActionID { get; set; }
        [DataMember]
        public int? PlanID { get; set; }
        [DataMember]
        public string AuditDate { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public string AuditArea{ get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string StateName { get; set; }
        [DataMember]
        public MAuditAreaItems[] Areas { get; set; }
    }
}
