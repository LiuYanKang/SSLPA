using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    public class QQualitCheckInfo : PagerParams
    {
        public string Keyword { get; set; }
        public int? MaterialType { get; set; }
        public int ItemID { get; set; }
        public int ItemType { get; set; }
        public int ProcessID { get; set; }

    }
}
