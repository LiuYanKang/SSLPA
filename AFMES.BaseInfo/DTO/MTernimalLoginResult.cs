using SeekerSoft.Core.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    /// <summary>
    /// 终端登录返回模型
    /// </summary>
    [DataContract]
    public class MTernimalLoginResult
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        [DataMember]
        public UserInfo UserInfo { get; set; }
        /// <summary>
        /// 当前登录设备信息
        /// </summary>
        [DataMember]
        public MMachine Machine { get; set; }
    }
}
