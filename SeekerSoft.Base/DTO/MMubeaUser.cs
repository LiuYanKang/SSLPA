using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    /// <summary>
    /// 慕贝尔用户映射
    /// </summary>
    [DataContract]
    public class MMubeaUser
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMember]
        public string User_Name { get; set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        [DataMember]
        public string User_Password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [DataMember]
        public string Email { get; set; }
    }
}
