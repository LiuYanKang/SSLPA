using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MReport
    {
        [DataMember]
        public int ProbCount { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
