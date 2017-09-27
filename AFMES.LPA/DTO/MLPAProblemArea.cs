using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    [DataContract]
    public class MLPAProblemArea
    {
        /// <summary>
        /// 月份
        /// </summary>
        [DataMember]
        public int? AreaMonth { get; set; }

        /// <summary>
        /// 问题数量
        /// </summary>
        [DataMember]
        public int? ProblemCount { get; set; }

        /// <summary>
        /// 关闭数量
        /// </summary>
        [DataMember]
        public double? CloseCount { get; set; }

        [DataMember]
        public double? ProblemCountSum { get; set; }

        [DataMember]
        public double? CloseCountSum { get; set; }

        /// <summary>
        /// 问题关闭率
        /// </summary>
        [DataMember]
        public double? CloseRate { get; set; }

        /// <summary>
        /// 第几周
        /// </summary>
        [DataMember]
        public int? Weeks { get; set; }

        /// <summary>
        /// 问题分类名称
        /// </summary>
        [DataMember]
        public string  Name { get; set; }





        [DataMember]
        public int? PlanCountAll { get; set; }
        [DataMember]
        public double? PlanCountFinished { get; set; }
        [DataMember]
        public double? PlanFinishedRate { get; set; }












    }

    /// <summary>
    /// 问题关闭率周报表
    /// </summary>
    [DataContract]
    public class WeeksReport
    {
        [DataMember]
        public string[] Weeks { get; set; }

        [DataMember]
        public MLPAProblemArea[] Data { get; set; }
    }

    [DataContract]
    public class ProCateGroyWkReport
    {
        /// <summary>
        /// 周次
        /// </summary>
        [DataMember]
        public string[] Weeks { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [DataMember]
        public MLPAProblemArea[] Data { get; set; }

        /// <summary>
        /// 问题分类
        /// </summary>
        [DataMember]
        public string[] ProCategroyList { get; set; }

        /// <summary>
        /// 人员列表
        /// </summary>
        [DataMember]
        public string[] NameList { get; set; }


        [DataMember]
        public MProSum[] ProSum { get; set; }

    }


}
