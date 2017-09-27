using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class MMenu
    {
        [DataMember]
        public string MenuCode { get; set; }
        [DataMember]
        public string PCode { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Icon { get; set; }  
        [DataMember]
        public string Color { get; set; }  
        [DataMember]
        public bool Visible { get; set; }  
    }
}
