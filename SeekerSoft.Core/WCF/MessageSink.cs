using SeekerSoft.Core.Auth;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace SeekerSoft.Core.WCF
{
    public class MessageSink : IMessageSink
    {
        private Type _bllClassType;
        private ServiceBase _bllObject;
        private IMessageSink _NextSink;


        public MessageSink(Type bllClassType, ServiceBase bllObj, IMessageSink nextSink)
        {
            _bllClassType = bllClassType;
            _bllObject = bllObj;
            this._NextSink = nextSink;
        }

        #region IMessageSink 成员

        public IMessageSink NextSink { get { return this._NextSink; } }

        //IMessageSink的接口方法，当消息传递的时被调用
        public IMessage SyncProcessMessage(IMessage msg)
        {
            IMethodCallMessage call = msg as IMethodCallMessage;

            var userinfo = FillUserInfo(call);
            IMessage msgException;
            if (!ValidateAuth(userinfo, call, out msgException))
            {
                return msgException;
            }

            //传递消息给下一个接收器
            var retMsg = this._NextSink.SyncProcessMessage(msg);
            var errMsg = retMsg as System.Runtime.Remoting.Messaging.ReturnMessage;
            if (errMsg != null && errMsg.Exception != null)
            {
                return ThrowException(call, errMsg.Exception.Message, ResultStatus.Error);
            }
            return retMsg;
        }

        //IMessageSink接口方法，用于异步处理，权限验证中不需要异步所以返回null
        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            return null;
        }

        #endregion


        /// <summary>
        /// 返回异常消息
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        private IMessage ThrowException(IMethodCallMessage call, string ex, ResultStatus status = ResultStatus.Error)
        {
            MethodInfo methodInfo = _bllClassType.GetMethod(call.MethodName);
            Object Result = Activator.CreateInstance(methodInfo.ReturnType);
            PropertyInfo pi = null;
            pi = methodInfo.ReturnType.GetProperty("Status");
            pi.SetValue(Result, status, null);
            var addmethod = methodInfo.ReturnType.GetMethod("AddMsg");
            addmethod.Invoke(Result, new object[] { ex });
            return new ReturnMessage(Result, null, 0, null, call);
        }

        /// <summary>
        ///  填充用户信息
        /// </summary>
        /// <param name="call"></param>
        private UserInfo FillUserInfo(IMethodCallMessage call)
        {
            ParameterInfo[] parameters = call.MethodBase.GetParameters();
            if (parameters == null || parameters.Length == 0
                || !"ticket".Equals(parameters[0].Name, StringComparison.InvariantCultureIgnoreCase)
                || call.InArgs[0] == null)
                return null;

            Guid ticket = (Guid)call.InArgs[0];
            var userinfo = UserCache.GetByTicket(ticket);
            _bllClassType.BaseType.GetProperty("UserInfo").SetValue(_bllObject, userinfo, null);
            return userinfo;
        }
        /// <summary>
        /// 验证权限
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="call"></param>
        private bool ValidateAuth(UserInfo userinfo, IMethodCallMessage call, out IMessage exceptionMsg)
        {
            exceptionMsg = null;

            var needFuncs = call.MethodBase.GetCustomAttributes(typeof(AuthAttribute), false) as AuthAttribute[];
            if (needFuncs == null || needFuncs.Length == 0) return true;

            if (userinfo == null)
            {// 未登录
                exceptionMsg = ThrowException(call, "未登录", ResultStatus.LoginOut);
                return false;
            }

            if (ValidateFuncs(needFuncs, userinfo)) return true;

            exceptionMsg = ThrowException(call, "没有权限", ResultStatus.NoRight);
            return false;
        }


        /// <summary>
        /// 验证权限(满足其中一个即通过)
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool ValidateFuncs(AuthAttribute[] limitList, UserInfo userinfo)
        {
            if (userinfo == null) return false;

            foreach (var auth in limitList)
            {
                if (string.IsNullOrWhiteSpace(auth.FuncCode)) return true;
                if (userinfo.AuthList.Any(func => func == auth.FuncCode))
                    return true;
            }
            return false;
        }

    }
}
