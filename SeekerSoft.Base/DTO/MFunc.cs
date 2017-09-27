using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class MFunc
    {
        [DataMember]
        public string FuncCode { get; set; }
        [DataMember]
        public string ParentCode { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}
