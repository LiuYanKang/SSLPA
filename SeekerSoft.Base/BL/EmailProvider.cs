using SeekerSoft.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SeekerSoft.Base.BL
{
    public class EmailProvider
    {
        public static readonly bool Email_Enable = ConfigHelper.Get("Email_Enable") == "1";
        public static readonly string Email_Server = ConfigHelper.Get("Email_Server");
        public static readonly string Email_Account = ConfigHelper.Get("Email_Account");
        public static readonly string Email_Pwd = ConfigHelper.Get("Email_Pwd");


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mobile">邮箱地址</param>
        /// <param name="msg">邮件内容</param>
        public static string SendEmail(string receiver, string subject, string msg, bool isHTML = false)
        {
            string content = "";
            if (string.IsNullOrEmpty(receiver))
            {
                content = "邮件地址为空";
                return content;
            }
            if (Email_Enable)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(receiver);
                mail.From = new MailAddress(Email_Account, "MES SYSTEM", Encoding.UTF8);
                mail.Subject = subject;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = msg;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = isHTML;
                mail.Priority = MailPriority.High;

                SmtpClient client = new SmtpClient();
                if (!string.IsNullOrEmpty(Email_Pwd))
                    client.Credentials = new System.Net.NetworkCredential(Email_Account, Email_Pwd);

                client.Host = Email_Server;
                object userState = mail;
                try
                {
                    client.SendAsync(mail, userState);
                    content = "发送成功";
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    content = "发送邮件出错:" + ex.Message;
                }
            }
            else
            {
                content = "系统配置为不发送邮件";
            }

            //记录发送记录
            using (var db = DB.DBHelper.NewDB())
            {
                if (msg.Length > 1500)
                    msg = msg.Substring(0, 1500);
                db.EmailSend.Add(new DB.EmailSend()
                {
                    Receiver = receiver,
                    Content = subject + ":" + msg,
                    SendTime = DateTime.Now,
                    ResultData = content
                });
                db.SaveChanges();
            }

            return content;
        }
    }
}
