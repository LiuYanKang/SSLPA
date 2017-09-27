using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class MLoginUser
    {
        [DataMember]
        public int? UserID { get; set; }
        [DataMember]
        public int EmpID { get; set; }
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public string Pwd { get; set; }
        [DataMember]
        public bool IsDisabled { get; set; }
        [DataMember]
        public bool IsDel { get; set; }
    }
}
