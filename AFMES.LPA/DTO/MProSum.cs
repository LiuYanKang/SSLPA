using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{



    [DataContract]
    public class MProSum

    {
        [DataMember]
        public int? AreaMonth { get; set; }

        [DataMember]
        public int? ProblemCountSum { get; set; }

        [DataMember]
        public double? CloseCountSum { get; set; }

    }
}
