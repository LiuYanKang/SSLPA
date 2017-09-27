using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MSetActionPlan
    {
        /// <summary>
        /// 审核类型
        /// </summary>
        [DataMember]
        public string AuditType { get; set; }

        /// <summary>
        /// 审核区域
        /// </summary>
       [DataMember]
       public string[] AuditArea { get; set; }
    }
}
