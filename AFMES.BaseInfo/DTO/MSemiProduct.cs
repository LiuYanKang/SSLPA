using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MSemiProduct
    {
        [DataMember]
        public int? SemiProdID { get; set; }
        [DataMember]
        public int SupplierID { get; set; }
        [DataMember]
        public Nullable<int> RawMaterialID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public double Diameter { get; set; }
        [DataMember]
        public string Strength { get; set; }
        [DataMember]
        public double MaxStorage { get; set; }
        [DataMember]
        public double MinStorage { get; set; }
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public string SupplierName { get; set; }
        [DataMember]
        public string MaterialCode { get; set; }
        [DataMember]
        public string SupplierCode { get; set; }
        [DataMember]
        public string StrengthName { get; set; }
    }
}
