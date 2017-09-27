using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MLpaSection
    {
        [DataMember]
        public int? PlanID { get; set; }

        [DataMember]
        public int EmpID { get; set; }

        [DataMember]
        public DateTime? StartPlanDate { get; set; }

        [DataMember]
        public DateTime? EndPlanDate { get; set; }

        [DataMember]
        public string WeekState { get; set; }

        [DataMember]
        public string Section { get; set; }

        [DataMember]
        public string Auditor { get; set; }

        [DataMember]
        public string Period { get; set; }

        [DataMember]
        public string AuditType { get; set; }
    }
}
