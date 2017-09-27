using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MStockLocation
    {
        [DataMember]
        public int? LocID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string StockType { get; set; }
        [DataMember]
        public string BoxNo { get; set; }
        [DataMember]
        public Nullable<int> MaterialID { get; set; }
        [DataMember]
        public string PN { get; set; }
        [DataMember]
        public Nullable<int> Quantity { get; set; }
        [DataMember]
        public Nullable<double> Weight { get; set; }
        [DataMember]
        public Nullable<System.DateTime> InOutTime { get; set; }
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public string StockTypeName { get; set; }
        [DataMember]
        public double? Diameter { get; set; }
    }
}
