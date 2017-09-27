using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MAuditGroup
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public MAuditItem[] Items { get; set; }
    }
}
