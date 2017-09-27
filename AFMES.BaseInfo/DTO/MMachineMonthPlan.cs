using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MMachineMonthPlan
    {
        [DataMember]
        public int MachineID { get; set; }
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public int PYear { get; set; }
        [DataMember]
        public string PYearName { get; set; }
        [DataMember]
        public int PMonth { get; set; }
        [DataMember]
        public string PMonthName { get; set; }
        [DataMember]
        public double QTY { get; set; }
        [DataMember]
        public string Remark { get; set; }

    }
}
