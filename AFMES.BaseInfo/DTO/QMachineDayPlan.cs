using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    public class QMachineDayPlan : PagerParams
    {
        public string Keyword { get; set; }
        public int? MachineID { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
