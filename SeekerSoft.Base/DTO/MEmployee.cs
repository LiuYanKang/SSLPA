using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class MEmployee
    {
        [DataMember]
        public int? EmpID { get; set; }
        [DataMember]
        public int DeptID { get; set; }
        [DataMember]
        public Nullable<int> UserID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string EmpCode { get; set; }
        [DataMember]
        public string Gender { get; set; }
        [DataMember]
        public string GenderName { get; set; }
        [DataMember]
        public string Tel { get; set; }
        [DataMember]
        public string EMail { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string StatusName { get; set; }
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
        public string DepartmentName { get; set; }
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public string NFCID { get; set; }
        [DataMember]
        public string PhotoFile { get; set; }

        /// <summary>
        /// 生产部门代码
        /// </summary>
        [DataMember]
        public string PDCode { get; set; }

     

    }
}
