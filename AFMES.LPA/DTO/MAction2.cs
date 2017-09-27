using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MAction2
    {
        [DataMember]
        public int? ActionID { get; set; }
        [DataMember]
        public Nullable<int> PlanID { get; set; }
        [DataMember]
        public int EmpID { get; set; }
        [DataMember]
        public string EmpName { get; set; }
        [DataMember]
        public System.DateTime AuditDate { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public string AuditArea { get; set; }
        [DataMember]
        public string AuditAreaName { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string StateName { get; set; }
        

        [DataMember]
        public MLPAActionResult[] LPAActionResultList { get; set; }
        [DataMember]
        public MProblem[] ProblemList { get; set; }



    }
}
