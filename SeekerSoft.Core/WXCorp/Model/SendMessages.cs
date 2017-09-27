using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.WXCorp.Model
{
    public class SendMessages
    {
        /// <summary>
        /// 非必填  员工ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为@all，则向关注该企业应用的全部成员发送
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 非必填 部门ID列表，多个接收者用‘|’分隔，最多支持100个。当touser为@all时忽略本参数
        /// </summary>
        public string toparty { get; set; }

        /// <summary>
        /// 非必填 标签ID列表，多个接收者用‘|’分隔。当touser为@all时忽略本参数
        /// </summary>
        public string totag { get; set; }


        /// <summary>
        /// 必填值 消息类型，此时固定为：text
        /// </summary>
        public string msgtype { get; set; }


        /// <summary>
        /// 企业应用的id，整型。可在应用的设置页面查看
        /// </summary>
        public string agentid { get; set; }


        /// <summary>
        /// 消息内容
        /// </summary>
        public Content text { get; set; }


        /// <summary>
        /// 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public string safe { get; set; }

        /// <summary>
        /// 新闻内容
        /// </summary>
        public Content news { get; set; }
    }

    public class Content
    {

        public string content { get; set; }

        public Article[] articles { get; set; }
    }

    public class Article
    {
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string picurl { get; set; }
    }
}
