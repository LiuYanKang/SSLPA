using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class MRole
    {
        [DataMember]
        public int RoleID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Remark { get; set; }
    }
}
