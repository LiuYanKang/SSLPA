using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MQualitCheckInfo
    {
        [DataMember]
        public int? QCInfoID { get; set; }
        [DataMember]
        public int ProcessID { get; set; }
        [DataMember]
        public int ItemID { get; set; }
        [DataMember]
        public string ItemType { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string InfoType { get; set; }
        [DataMember]
        public string InfoTypeName { get; set; }
        [DataMember]
        public Nullable<double> MaxValue { get; set; }
        [DataMember]
        public Nullable<double> MinValue { get; set; }


        [DataMember]
        public int MaterialID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string MaterialType { get; set; }

    }
}
