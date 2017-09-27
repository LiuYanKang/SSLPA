using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MDownCode
    {
        [DataMember]
        public int? DownCodeID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public Nullable<int> PID { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}
