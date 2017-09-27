using SeekerSoft.Core.Config;
using SeekerSoft.Core.WXCorp.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Xml;

namespace SeekerSoft.Core.WXCorp
{
    public class WXCorpHelper
    {
        private static string AccessToken = string.Empty;
        private static DateTime TokenYXQ;// Token 有效期
        /// <summary>  
        /// 根据当前日期 判断Access_Token 是否超期  如果超期返回新的Access_Token   否则返回之前的Access_Token  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static string GetToken()
        {
            if (AccessToken == null || DateTime.Now > TokenYXQ)
            {
                string CorpID = ConfigHelper.Get("CorpID");
                string Secret = ConfigHelper.Get("Secret");
                var tokenModel = GetToken(CorpID, Secret);
                AccessToken = tokenModel.access_token;
                TokenYXQ = DateTime.Now.AddSeconds(tokenModel.expires_in);
            }
            return AccessToken;
        }

        private static string JSAPITicket;
        private static DateTime JSAPITicketYXQ;// Ticket有效期

        public static string GetJSApiTicket()
        {
            if (JSAPITicket == null || DateTime.Now > JSAPITicketYXQ)
            {
                var url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token={0}", GetToken());
                JSAPITicketResult result = CorpManage.GetJson<JSAPITicketResult>(url);
                JSAPITicket = result.ticket;
                JSAPITicketYXQ = DateTime.Now.AddSeconds(result.expires_in);
            }

            return JSAPITicket;
        }

        public static string GetJSAPISignature(string nonceStr, string timestamp, string url)
        {
            string str = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}",
                GetJSApiTicket(),
                nonceStr,
                timestamp,
                url);

            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1");
        }

        public static long GetTimeStamp()
        {
            return DateTime.Now.Ticks / 1000 / 1000 / 100;
        }

        /// <summary>
        /// 获取当前登录人的UserID
        /// </summary>
        /// <param name="code"></param>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public static string GetUserid(string code, string agentid)
        {
            string access_token = GetToken();
            var url =
                "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token=" + access_token + "&code=" + code + "&agentid=" + agentid;
            AccessTokenResults result = CorpManage.GetJson<AccessTokenResults>(url);
            return result.UserId;
        }

        /// <summary>
        /// 获取凭证接口
        /// </summary>
        /// <param name="grant_type">获取access_token填写client_credential</param>
        /// <param name="appid">第三方用户唯一凭证</param>
        /// <param name="secret">第三方用户唯一凭证密钥，既appsecret</param>
        /// <returns></returns>
        public static AccessTokenResult GetToken(string CorpID, string Secret)
        {
            var url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}",
                                   CorpID, Secret);
            AccessTokenResult result = CorpManage.GetJson<AccessTokenResult>(url);
            return result;
        }
    }
}
