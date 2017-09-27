using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    public class QMachine : PagerParams
    {
        public string Keyword { get; set; }
        public int? ProcID { get; set; }
        public string ProductType { get; set; }
    }
}

