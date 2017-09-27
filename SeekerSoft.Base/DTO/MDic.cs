using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class MDic
    {
        [DataMember]
        public string DicCode { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }

    [DataContract]
    public class MDicItem
    {
        [DataMember]
        public string DicCode { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsSys { get; set; }
        [DataMember]
        public int SN{ get; set; }
        [DataMember]
        public string Remark { get; set; }
    }

}
