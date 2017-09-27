using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class QLpaOverviewWeek : PagerParams
    {
        /// <summary>
        /// 年份
        /// </summary>
        [DataMember]
        public int ByYear { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        [DataMember]
        public int ByWeek { get; set; }
    }
}
