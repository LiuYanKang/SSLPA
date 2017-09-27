using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SeekerSoft.Base.DTO
{
    [DataContract]
    public class MUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public int UserID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMember]
        public string LoginName { get; set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        [DataMember]
        public string Pwd { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [DataMember]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
    }
}
