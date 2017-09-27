using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.WXCorp.Model
{
    public class WXMsgResult
    {
        public ReturnCode? errcode { get; set; }
        public string errmsg { get; set; }

    }

    public class WXMsgResultDept : WXMsgResult
    {
        public List<WXOrg> department { get; set; }
    }

    public class WXMsgResultUser : WXMsgResult
    {
        public List<WXUser> userlist { get; set; }
    }

    public class WXMsgResultDeptCreate : WXMsgResult
    {
        public int? id { get; set; }
    }
}
