using SeekerSoft.Base.DB;
using SeekerSoft.Base.DTO;
using SeekerSoft.Core;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.Config;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.Security;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SeekerSoft.Base.BL
{
    public class SMSProvider
    {
        public static readonly bool SMS_Enable = ConfigHelper.Get("SMS_Enable") == "1";
        public static readonly string SMS_SrvAddr = ConfigHelper.Get("SMS_SrvAddr");
        public static readonly string SMS_Account = ConfigHelper.Get("SMS_Account");
        public static readonly string SMS_Key = ConfigHelper.Get("SMS_Key");


        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="msg">消息内容</param>
        public static string SendSMS(string mobile, string msg)
        {
            try
            {
                string content = "";
                if (!ValidTel(mobile))
                {
                    content = "手机号码不正确";
                }
                else
                if (SMS_Enable)
                {
                    // 加密
                    string sign = MD5Helper.Sign(SMS_Account + mobile + msg, SMS_Key, Encoding.UTF8);

                    var sb = new StringBuilder();
                    sb.AppendFormat(SMS_SrvAddr,
                        sign,
                        SMS_Account,
                        mobile,
                        msg);
                    var uri = new Uri(sb.ToString());
                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Accept = "application/json";
                    request.Method = "GET";
                    var response = (HttpWebResponse)request.GetResponse();
                    var streamResponse = response.GetResponseStream();
                    var streamRead = new StreamReader(streamResponse, Encoding.UTF8);
                    var readBuff = new Char[256];
                    int count = streamRead.Read(readBuff, 0, 256);

                    while (count > 0)
                    {
                        var outputData = new string(readBuff, 0, count);
                        content += outputData;
                        count = streamRead.Read(readBuff, 0, 256);
                    }
                    if (content != "{\"d\":\"success\"}")
                        Log.Debug(typeof(SMSProvider).ToString(), "发送短信失败：" + content + "——" + mobile + "：" + msg);

                }
                else
                {
                    content = "系统配置为不发送短信";
                }

                // 记录发送记录
                using (var db = DB.DBHelper.NewDB())
                {
                    db.SMSSend.Add(new DB.SMSSend()
                    {
                        Tel = mobile,
                        Content = msg,
                        SendTime = DateTime.Now,
                        ResultData = content
                    });
                    db.SaveChanges();
                }

                return content;
            }
            catch (Exception ex)
            {
                Log.Debug(typeof(SMSProvider).ToString(), "短信发送出现异常：" + ex.ToString());
                return ex.Message;
            }
        }

        private static bool ValidTel(string mobile)
        {
            if (mobile == null || mobile.Length != 11)
                return false;

            return true;
        }



        public static PagerResult<MSMSSend> SMSSendQuery(UserInfo userinfo, QSMSSend param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT  LogID ,
        Tel ,
        Content ,
        SendTime ,
        ResultData FROM base.SMSSend
        WHERE 1=1
        ");
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat(" AND Tel LIKE '%{0}%' OR Content LIKE '%{0}%'", param.Keyword.Replace("'", "''"));
            }
            if (param.StartTime != null)
            {
                sql.AppendFormat(@" AND SendTime >= '{0}' ", param.StartTime.Value.Date);
            }
            if (param.EndTime != null)
            {
                sql.AppendFormat(@" AND SendTime < '{0}' ", param.EndTime.Value.Date.AddDays(1));
            }
            // 排序
            if (string.IsNullOrEmpty(param.sortField))
            {
                param.sortField = "SendTime";
                param.sortOrder = "DESC";
            }

            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                return DBHelper<MSMSSend>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
            }
        }

        public static MSMSSend SMSSendGet(int id)
        {
            string sql = @"SELECT  LogID ,
        Tel ,
        Content ,
        SendTime ,
        ResultData FROM base.SMSSend
		WHERE LogID= " + id;

            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MSMSSend>.ExecuteSqlToEntities(db, sql).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");
                return model;
            }
        }

    }
}
