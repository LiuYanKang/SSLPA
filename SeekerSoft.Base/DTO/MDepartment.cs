using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class MDepartment
    {
        [DataMember]
        public int? DeptID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Nullable<int> PID { get; set; }

        [DataMember]
        public string PName { get; set; }

        [DataMember]
        public int? ManagerId { get; set; }

        [DataMember]
        public string ManagerName { get; set; }

        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public bool IsDel { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreateTime { get; set; }
        [DataMember]
        public Nullable<int> CreateBy { get; set; }
        [DataMember]
        public Nullable<System.DateTime> UpdateTime { get; set; }
        [DataMember]
        public Nullable<int> ModifyBy { get; set; }

        [DataMember]
        public Nullable<int> SN { get; set; }

        [DataMember]
        public string FullDeptID { get; set; }
        
    }
}
