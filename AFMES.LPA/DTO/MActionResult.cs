using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MActionResult
    {
        [DataMember]
        public int ActionID { get; set; }
        [DataMember]
        public int ItemID { get; set; }
        [DataMember]
        public int Result { get; set; }
        [DataMember]
        public string EnterNum { get; set; }
    }
}
