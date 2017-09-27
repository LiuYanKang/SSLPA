using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.DTO
{
    public class QEmployee : PagerParams
    {
        public string Keyword { get; set; }
        public string Status { get; set; }
    }
}
