using SeekerSoft.Core.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace SeekerSoft.Core.WCF
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class AuthAttribute : Attribute
    {
        public AuthAttribute()
        {
        }

        public AuthAttribute(string funcCode)
        {
            FuncCode = funcCode;

        }

        /// <summary>
        /// 所需要的功能代码
        /// </summary>
        public string FuncCode { get; set; }
    }
}
