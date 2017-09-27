using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace SeekerSoft.Core.WCF
{
    /// <summary>
    /// 标记服务启用权限验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AuthValidServiceAttribute : ContextAttribute
    {
        public AuthValidServiceAttribute() : base("AuthValidServiceAttribute") { }

        /// <summary>
        /// 将当前上下文属性添加到给定的消息。
        /// </summary>
        /// <param name="ctorMsg"></param>
        public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
        {
            //实例化一个ContextAccessPowerValidProperty 添加到上下文属性列表中
            ctorMsg.ContextProperties.Add(new AuthValidProperty(ctorMsg.ActivationType));
        }
    }
}
