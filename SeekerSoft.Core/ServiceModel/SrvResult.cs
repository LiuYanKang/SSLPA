using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SeekerSoft.Core.ServiceModel
{
    /// <summary>
    /// 服务返回结果包装类
    /// </summary>
    /// <typeparam name="T">返回结果类型</typeparam>
    public class SrvResult<T>
    {
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ResultStatus Status { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [DataMember]
        public T Data { get; set; }


        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        public string Msg { get; set; }

        /// <summary>
        /// 信息列表
        /// </summary>
        [DataMember]
        public List<String> MsgList { get; set; }

        /// <summary>
        /// 添加一条消息
        /// </summary>
        /// <param name="msg"></param>
        public void AddMsg(string msg)
        {
            Msg = msg;
            if (MsgList == null) MsgList = new List<string>();
            MsgList.Add(msg);
        }
    }


    /// <summary>
    /// 执行结果
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// 没有权限
        /// </summary>
        NoRight = -2,

        /// <summary>
        /// 登入超时（未登入）
        /// </summary>
        LoginOut = -1,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 频繁操作
        /// </summary>
        Frequently = 2,
    }
}