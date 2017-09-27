using SeekerSoft.Core.Config;
using SeekerSoft.Core.WXPP.Model;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace SeekerSoft.Core.WXPP
{
    public class WXPPHelper
    {
        public static readonly string Appid = ConfigHelper.Get("WXAppID");
        public static readonly string Secret = ConfigHelper.Get("WXSecretCode");

        public static void Init()
        {
            AccessTokenContainer.Register(Appid, Secret);
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            return AccessTokenContainer.GetAccessToken(Appid);
        }
        /// <summary>
        /// 获取JsApiTicket
        /// </summary>
        /// <returns></returns>
        public static string GetJsApiTicket()
        {
            return AccessTokenContainer.GetJsApiTicket(Appid);
        }

        /// <summary>
        /// 获取openid 根据授权页面重定向后的code
        /// </summary>
        public static string GetOpenIDByCode(string code)
        {
            return OAuthApi.GetAccessToken(Appid, Secret, code).openid;
        }

    }
}
