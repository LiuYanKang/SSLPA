using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MEmployee
    {
        [DataMember]
        public int EmpID { get; set; }
        [DataMember]
        public string EmpName { get; set; }
        [DataMember]
        public string Position { get; set; }
        [DataMember]
        public Nullable<int> SuperiorID { get; set; }
        [DataMember]
        public string SuperiorName { get; set; }
        [DataMember]
        public bool IsResponsible { get; set; }

        /// <summary>
        /// 审核区域
        /// </summary>
        [DataMember]
        public string[] AuditArea { get; set; }
        /// <summary>
        /// 审核区域
        /// </summary>
        [DataMember]
        public string AuditAreaSummary { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        [DataMember]
        public string AuditType { get; set; }
        /// <summary>
        /// 审核类型名称
        /// </summary>
        [DataMember]
        public string AuditTypeName { get; set; }

        /// <summary>
        /// Section
        /// </summary>
        [DataMember]
        public string Section { get; set; }
    }
}
