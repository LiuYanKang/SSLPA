using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace SeekerSoft.Core.WCF
{
    public class AuthValidProperty : IContextProperty, IContributeObjectSink
    {
        public AuthValidProperty()
        {
        }
        /// <summary>
        /// 业务类类型
        /// </summary>
        private Type _bllClassType = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bllClassType">业务类类型</param>
        public AuthValidProperty(Type bllClassType)
        {
            _bllClassType = bllClassType;
        }

        #region IContextProperty 成员

        public void Freeze(Context newContext)
        {
        }
        public bool IsNewContextOK(Context newCtx)
        {
            return true;
        }

        public string Name
        {
            get { return "AccessPowerValid"; }
        }

        #endregion

        #region IContributeObjectSink 成员

        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
        {
            return new MessageSink(_bllClassType, (ServiceBase)obj, nextSink);
        }
        #endregion
    }
}
