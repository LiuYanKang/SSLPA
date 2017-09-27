using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MProblemPic
    {
        [DataMember]
        public int? PicID { get; set; }
        [DataMember]
        public int ProbID { get; set; }
        [DataMember]
        public string PicType { get; set; }
        [DataMember]
        public string FileName { get; set; }
    }
}
