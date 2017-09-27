using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MWorkingShift
    {
        [DataMember]
        public int? ShiftID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public System.DateTime BeginTime { get; set; }
        [DataMember]
        public System.DateTime EndTime { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}
