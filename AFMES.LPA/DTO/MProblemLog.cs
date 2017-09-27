using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MProblemLog
    {
        [DataMember]
        public int LogID { get; set; }
        [DataMember]
        public int ProbID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> PlanEndDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> NewPlanEndDate { get; set; }
    }
}
