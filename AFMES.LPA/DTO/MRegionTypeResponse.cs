using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MRegionTypeResponse
    {
        [DataMember]
        public MArea[] ItemsRegion { get; set; }
        [DataMember]
        public SeekerSoft.Base.DTO.MDicItem[] ItemsType { get; set; }
        [DataMember]
        public MEmployee[] Emps { get; set; }
    }
}
