using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class QLPAProblemArea
    {
        /// <summary>
        /// 年份
        /// </summary>
        [DataMember]
        public string ByYear { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        [DataMember]
        public string ByMonth { get; set; }

        /// <summary>
        /// 工序ID必要条件
        /// </summary>
        [DataMember]
        public int? AreaId { get; set; }
    }
}
