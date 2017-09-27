using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
  public  class MProGroup
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string SuperEmail { get; set; }
        [DataMember]
        public string SuperName { get; set; }
        [DataMember]
        public MProblem[] Items { get; set; }
    }
}
