using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MExpired
    {
        [DataMember]
        public int? Total { get; set; }
        [DataMember]
        public string EndDate { get; set; }
        [DataMember]
        public int EmpID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string SuperName { get; set; }
        [DataMember]
        public string SuperEmail { get; set; }
        [DataMember]
        public int Expired { get; set; }
    }
}
