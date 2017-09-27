using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MRawMaterial
    {
        [DataMember]
        public int? RawMaterialID { get; set; }
        [DataMember]
        public int SupplierID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public double Diameter { get; set; }
        [DataMember]
        public double MaxStorage { get; set; }
        [DataMember]
        public double MinStorage { get; set; }
        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public string SupplierName { get; set; }
    }
}
