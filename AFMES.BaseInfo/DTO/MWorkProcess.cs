using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MWorkProcess
    {
        [DataMember]
        public int? ProcID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ProcType { get; set; }
        [DataMember]
        public string ProcTypeName { get; set; }
        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public string PDCode { get; set; }
    }
}
