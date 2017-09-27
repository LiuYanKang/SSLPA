using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MActionPlan
    {
        [DataMember]
        public int? PlanID { get; set; }
        [DataMember]
        public string AuditType { get; set; }
        [DataMember]
        public string AuditTypeName  { get; set; }
        [DataMember]
        public string StartPlanDate { get; set; }
        [DataMember]
        public string EndPlanDate { get; set; }
    }
}
