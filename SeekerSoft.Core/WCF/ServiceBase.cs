using SeekerSoft.Core.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.WCF
{
    /// <summary>
    /// WCF服务基类
    /// </summary>
    public class ServiceBase : ContextBoundObject
    {
        public static UserInfo UserInfo { get; set; }

    }
}
