using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MAuditAreaItems
    {
        [DataMember]
        public string AuditArea { get; set; }
        [DataMember]
        public string AreaName { get; set; }
        [DataMember]
        public MAuditItem[] Items { get; set; }
    }
}
