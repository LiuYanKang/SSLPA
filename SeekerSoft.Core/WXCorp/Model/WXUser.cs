using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.WXCorp.Model
{
    public class WXUser
    {
        /// <summary>
        /// 用户ID不能重复
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public List<int> department { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string position { get; set; }


        /// <summary>
        /// 手机号 ，不能重复
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 性别 1男 2女 0未知
        /// </summary>
        public int? gender { get; set; }


        /// <summary>
        /// 联系电话
        /// </summary>
        public string tel { get; set; }


        /// <summary>
        /// 邮箱，微信中不能重复
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 微信账号,微信中必须存在
        /// </summary>
        public string weixinid { get; set; }

        /// <summary>
        /// 关注状态: 1=已关注，2=已冻结，4=未关注
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 启用/禁用成员。1表示启用成员，0表示禁用成员
        /// </summary>
        public int? enable { get; set; }


    }
}
