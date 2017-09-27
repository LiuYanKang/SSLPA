using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MProblem
    {
        [DataMember]
        public int? ProbID { get; set; }
        [DataMember]
        public Nullable<int> ActionID { get; set; }
        [DataMember]
        public Nullable<int> ItemID { get; set; }
        [DataMember]
        public string ItemDesc { get; set; }
        [DataMember]
        public Nullable<int> PicID { get; set; }
        [DataMember]
        public int? ProblemRegion { get; set; }
        [DataMember]
        public string ProblemRegionName { get; set; }
        [DataMember]
        public string ProblemType { get; set; }
        [DataMember]
        public string ProblemTypeName { get; set; }
        [DataMember]
        public Nullable<int> MachineID { get; set; }
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public int? Responsible { get; set; }
        [DataMember]
        public string ResponsibleName { get; set; }
        [DataMember]
        public string ProblemDesc { get; set; }
        [DataMember]
        public DateTime? SubmitDate { get; set; }
        [DataMember]
        public DateTime? PlanStartDate { get; set; }
        [DataMember]
        public DateTime? PlanEndDate { get; set; }
        [DataMember]
        public DateTime? NewPlanEndDate { get; set; }
        [DataMember]
        public DateTime? OldPlanEndDate { get; set; }
        [DataMember]
        public DateTime? ActualEndDate { get; set; }
        [DataMember]
        public string Measure { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string StateName { get; set; }
        [DataMember]
        public int? Progress { get; set; }
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public string CreateByName { get; set; }

        [DataMember]
        public int? UserId { get; set; }

        [DataMember]
        public MProblemPic[] Images { get; set; }

        [DataMember]
        public MProblemPic[] BeforeProbPicList { get; set; }
        [DataMember]
        public MProblemPic[] AfterProbPicList { get; set; }
        [DataMember]
        public MProblemLog[] ProbLogList { get; set; }

        [DataMember]
        public DateTime? CreateTime { get; set; }




        [DataMember]
        public int? Total { get; set; }
        [DataMember]
        public string StartDate { get; set; }
        [DataMember]
        public string EndDate { get; set; }
        [DataMember]
        public int EmpID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string SuperName { get; set; }
        [DataMember]
        public string SuperEmail { get; set; }
        [DataMember]
        public int? Expired { get; set; }

        [DataMember]
        public string ImproveAdvice { get; set; }

    }
}
