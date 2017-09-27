using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.DTO
{
    public class QAction : PagerParams
    {
        public string Keyword { get; set; }
        public string State { get; set; }

        public string Area { get; set; }
    }
}
