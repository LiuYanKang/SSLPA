using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.DTO
{
    public class QActionPlan : PagerParams
    {
        public string Keyword { get; set; }
        public string Status { get; set; }
        public DateTime? PlanBeginDate { get; set; }
        public DateTime? PlanEndDate { get; set; }
        public DateTime? ActionBeginTime { get; set; }
        public DateTime? ActionEndTime { get; set; }
    }
}
