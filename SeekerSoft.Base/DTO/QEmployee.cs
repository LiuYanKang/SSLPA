using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    public class QEmployee : PagerParams
    {
        public string KeyWord { get; set; }
        public string Status { get; set; }

        public string DeptID { get; set; }

        public string FullDeptID { get; set; }

    }
}
