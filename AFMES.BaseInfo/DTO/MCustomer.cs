using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MCustomer
    {
        [DataMember]
        public int? CustID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Logo { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}
