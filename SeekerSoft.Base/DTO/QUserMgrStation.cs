﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class QUserMgrStation
    {
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public int[] StationID { get; set; }
    }
}
