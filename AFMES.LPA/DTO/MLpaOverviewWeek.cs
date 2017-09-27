using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.LPA.DTO
{
    /// <summary>
    /// 每周LPA计划视图
    /// </summary>
    [DataContract]
    public class MLpaOverviewWeek
    {
        /// <summary>
        /// 当前周日期
        /// </summary>
        [DataMember]
        public string[] WeekList { get; set; }

        /// <summary>
        /// SectionList
        /// </summary>
        [DataMember]
        public SectionList[] SectionList { get; set; }
    }

    /// <summary>
    /// 每组Section
    /// </summary>
    [DataContract]
    public class SectionList
    {
        /// <summary>
        /// Section名称
        /// </summary>
        [DataMember]
        public string SectionName { get; set; }

        /// <summary>
        /// 每组Section所有人员
        /// </summary>
        [DataMember]
        public EachPeopleList[] EachPeopleList { get; set; }


    }

    /// <summary>
    /// 每个人员
    /// </summary>
    [DataContract]
    public class EachPeopleList
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember]
        public string Auditor { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        [DataMember]
        public string Period { get; set; }

        /// <summary>
        /// 当前周每天的计划状态
        /// </summary>
        [DataMember]
        public string[] PeopleWeekState { get; set; }
    }

    /// <summary>
    /// 当前周每天的计划状态
    /// </summary>
    [DataContract]
    public class PeopleWeekState
    {
        /// <summary>
        /// 周一状态
        /// </summary>
        [DataMember]
        public string MonState { get; set; }

        /// <summary>
        /// 周二状态
        /// </summary>
        [DataMember]
        public string TueState { get; set; }

        /// <summary>
        /// 周三状态
        /// </summary>
        [DataMember]
        public string WedState { get; set; }

        /// <summary>
        /// 周四状态
        /// </summary>
        [DataMember]
        public string ThuState { get; set; }

        /// <summary>
        /// 周五状态
        /// </summary>
        [DataMember]
        public string FriState { get; set; }

        /// <summary>
        /// 周六状态
        /// </summary>
        [DataMember]
        public string SatState { get; set; }

        /// <summary>
        /// 周日状态
        /// </summary>
        [DataMember]
        public string SunState { get; set; }
    }

}
