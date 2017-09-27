using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MProdAbrMap
    {
        [DataMember]
        public int ProdID { get; set; }
        [DataMember]
        public string ProcCode { get; set; }
        [DataMember]
        public int ABRID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
