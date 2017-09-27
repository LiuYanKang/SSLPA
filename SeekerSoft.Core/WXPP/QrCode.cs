using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using SeekerSoft.Core.Config;
using SeekerSoft.Core.Json;

namespace SeekerSoft.Core.WXPP
{
    public class QrCode
    {
        public static string AccessToken = string.Empty;
        public static DateTime TokenYXQ;// Token 有效期

        public static readonly string Appid = ConfigHelper.Get("WXAppID");
        public static readonly string Secret = ConfigHelper.Get("WXSecretCode");

        /// <summary>
        /// 获取授权页面重定向后的openid  刷新access_token
        /// code
        /// </summary>
        public static string GetUserIDByOpenCode(string code)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + Appid + "&secret=" + Secret + "&code=" + code + "&grant_type=authorization_code");
            httpRequest.Timeout = 2000;
            httpRequest.Method = "GET";
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            var sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"));
            var result = sr.ReadToEnd();
            var json = JsonHelper.Deserialize<QrCodeInfo>(result);
            return (json.openid);
            //RefreshToken(json.refresh_token);
        }
    }

    public class QrCodeInfo
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string ticket { get; set; }

        public int total { get; set; }
        public int count { get; set; }
        public Auths data { get; set; }
        public string openid { get; set; }
        public string next_openid { get; set; }
        public string refresh_token { get; set; }


    }

    public class Auths
    {

        public string[] openid { get; set; }
    }
}
