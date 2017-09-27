using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MActionPlan2
    {
        [DataMember]
        public int? PlanID { get; set; }
        [DataMember]
        public int EmpID { get; set; }
        [DataMember]
        public string EmpName { get; set; }
        [DataMember]
        public System.DateTime StartPlanDate { get; set; }
        [DataMember]
        public System.DateTime EndPlanDate { get; set; }
        [DataMember]
        public string AuditType { get; set; }
        [DataMember]
        public string AuditTypeName { get; set; }
        [DataMember]
        public bool IsComplete { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ActionTime { get; set; }


        [DataMember]
        public int dayCount { get; set; }

        /// <summary>
        /// 审核区域
        /// </summary>
        [DataMember]
        public int[] AuditArea { get; set; }

        /// <summary>
        /// 审核区域
        /// </summary>
        [DataMember]
        public string AudtiAreaMes { get; set; }



        [DataMember]
        public int? BanCi { get; set; }

        [DataMember]
        public string BanCiName { get; set; }



        [DataMember]
        public int? Total { get; set; }
        [DataMember]
        public string StartDate { get; set; }
        [DataMember]
        public string EndDate { get; set; }
       
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string SuperName { get; set; }
        [DataMember]
        public string SuperEmail { get; set; }
        [DataMember]
        public int Expired { get; set; }

   
        

    }
}
