using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.DTO
{
    public class QProblem : PagerParams
    {
        public string Keyword { get; set; }
        public string State { get; set; }
        public int? MachineID { get; set; }
        public string ProblemRegionState { get; set; }
        public string ProblemTypeState { get; set; }

        public DateTime? SubmitBeginDate { get; set; }
        public DateTime? SubmitEndDate { get; set; }

        public int? Progress { get; set; }
    }
}
