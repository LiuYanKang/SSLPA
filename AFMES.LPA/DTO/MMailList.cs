using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
 public   class MMailList
    {
        [DataMember]
        public MActionPlan2[]  DataPlan { get; set; }


        [DataMember]
        public MProblem[] DataProblem { get; set; }


    }
}
