using SeekerSoft.Core.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Core.Auth
{
    [DataContract]
    public class UserInfo
    {
        /// <summary>
        /// 身份凭证
        /// </summary>
        [DataMember]
        public Guid Ticket { get; set; }

        /// <summary>
        /// 账户分类
        /// </summary>
        [DataMember]
        public UserType UserType { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public int UserID { get; set; }


        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public int DeptID { get; set; }
        
        /// <summary>
        /// 职员ID
        /// </summary>
        [DataMember]
        public int? EmpID { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        
        /// <summary>
        /// 授权功能列表
        /// </summary>
        [DataMember]
        public string[] AuthList { get; set; }

        /// <summary>
        /// 登陆终端
        /// 1网站 2手持终端
        /// </summary>
        [DataMember]
        public string Terminal { get; set; }
    }

    /// <summary>
    /// 账户分类
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// 员工
        /// </summary>
        Staff = 1
    }
}
