using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.DTO
{
    public class QAuditItem : PagerParams
    {
        public string Keyword { get; set; }
        public string AuditType { get; set; }
        public string AuditArea { get; set; }
        public string PDCode { get; set; }
    }
}
