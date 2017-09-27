using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Core.WXPP.Model
{
    public class JsAPISign
    {
        [DataMember]
        public string nonceStr { get; set; }
        [DataMember]
        public string timestamp { get; set; }

        [DataMember]
        public string signature { get; set; }
    }
}
