using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeekerSoft.Core.ServiceModel;

namespace SeekerSoft.Base.DTO
{
    public class QSMSSend : PagerParams
    {
        public string Keyword { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}

