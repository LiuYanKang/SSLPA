using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.WXCorp
{
    public class RequestXML
    {
        private string toUserName;
        /// <summary>
        /// 消息接收方微信号，一般为公众平台账号微信号
        /// </summary>
        public string ToUserName
        {
            get { return toUserName; }
            set { toUserName = value; }
        }

        private string fromUserName;
        /// <summary>
        /// 消息发送方微信号
        /// </summary>
        public string FromUserName
        {
            get { return fromUserName; }
            set { fromUserName = value; }
        }

        private string createTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }

        private string msgType;
        /// <summary>
        /// 信息类型 地理位置:location,文本消息:text,消息类型:image
        /// </summary>
        public string MsgType
        {
            get { return msgType; }
            set { msgType = value; }
        }

        private string content;
        /// <summary>
        /// 信息内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        private string msgid;
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MsgId
        {
            get { return msgid; }
            set { msgid = value; }
        }

        private string agentid;
        /// <summary>
        /// 企业应用id
        /// </summary>
        public string AgentID
        {
            get { return agentid; }
            set { agentid = value; }
        }

        private string location_X;
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public string Location_X
        {
            get { return location_X; }
            set { location_X = value; }
        }

        private string location_Y;
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public string Location_Y
        {
            get { return location_Y; }
            set { location_Y = value; }
        }

        private string scale;
        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public string Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        private string label;
        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        private string picUrl;
        /// <summary>
        /// 图片链接，开发者可以用HTTP GET获取
        /// </summary>
        public string PicUrl
        {
            get { return picUrl; }
            set { picUrl = value; }
        }

        private string mediaid;
        /// <summary>
        /// 图片消息媒体id
        /// </summary>
        public string MediaId
        {
            get { return mediaid; }
            set { mediaid = value; }
        }

        private string thumbmediaid;
        /// <summary>
        /// 视频消息缩略图id
        /// </summary>
        public string ThumbMediaId
        {
            get { return thumbmediaid; }
            set { thumbmediaid = value; }
        }
        public string Recognition { get; set; }
        public string EventName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string EventKey { get; set; }
        public string Event { get; set; }

        private string precision;
        ///<summary>
        ///地理位置精度 
        /// </summary>
        public string Precision
        {
            get { return precision; }
            set { precision = value; }
        }
    }

    public class AccessTokenResult
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }

    }
    public class JSAPITicketResult
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string ticket { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }

    }

    public class AccessTokenResults
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }

        public string refresh_token { get; set; }

        public string openid { get; set; }

        public string scope { get; set; }

        public string UserId { get; set; }
        public string DeviceId { get; set; }

    }
}
