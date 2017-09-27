using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MProdSemiMap
    {
        [DataMember]
        public int ProdID { get; set; }
        [DataMember]
        public int SemiProdID { get; set; }
    }
}
