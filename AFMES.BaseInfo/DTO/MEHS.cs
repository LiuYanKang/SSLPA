using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MEHS
    {
        [DataMember]
        public int? EHSID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Pic { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}
