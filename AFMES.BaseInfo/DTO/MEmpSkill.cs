using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MEmpSkill
    {

        [DataMember]
        public int EmpID { get; set; }
        [DataMember]
        public int MachineID { get; set; }
        [DataMember]
        public string MachineCode { get; set; }
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public string SkillLevel { get; set; }
        [DataMember]
        public string LevelName { get; set; }
        [DataMember]
        public string LevelRemark { get; set; }
    }
}
