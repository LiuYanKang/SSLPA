using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SeekerSoft.Core.Config;

namespace SSLPA.WebSite
{
    public class Global : System.Web.HttpApplication
    {
        //LPA触发器
        Timer lpaTimer;
        protected void Application_Start(object sender, EventArgs e)
        {
            // 从数据库中还原在线用户信息
            SeekerSoft.Base.BL.UserBL.InitOnlineUser();

            //LPA定点发送邮件
            string[] emailtime = ConfigHelper.Get("Email_Time").Split(':');
            DateTime LuckTime = DateTime.Now.Date.Add(new TimeSpan(Convert.ToInt32(emailtime[0]), Convert.ToInt32(emailtime[1]), Convert.ToInt32(emailtime[2])));
            TimeSpan span = LuckTime - DateTime.Now;
            if (span < TimeSpan.Zero)
            {
                span = LuckTime.AddDays(1d) - DateTime.Now;
            }
            lpaTimer = new Timer(new TimerCallback(obj =>
            {
                SSLPA.LPA.BL.ActionPlanBL.CheckExpired();
            }), null, span, TimeSpan.FromTicks(TimeSpan.TicksPerDay));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            //激活Application_Start，防止IIS应用程序池被回收  
            Thread.Sleep(1000);  
            string url = "index.html";
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            Stream receiveStream = myHttpWebResponse.GetResponseStream();
        }
    }
}