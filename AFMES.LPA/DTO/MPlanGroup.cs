using SSLPA.LPA.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA
{
    [DataContract]
    public class MPlanGroup
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string SuperEmail { get; set; }
        [DataMember]
        public string SuperName { get; set; }
        [DataMember]
        public MActionPlan2[] Items { get; set; }


        [DataMember]
        public int Ex { get; set; }
    }
}
