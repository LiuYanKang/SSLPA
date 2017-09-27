using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    public class MSMSSend
    {
        [DataMember]
        public long LogID { get; set; }
        [DataMember]
        public System.DateTime SendTime { get; set; }
        [DataMember]
        public string Tel { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string ResultData { get; set; }
    }
}
