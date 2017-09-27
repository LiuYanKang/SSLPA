using SeekerSoft.Core.Web;
using SeekerSoft.Core.WXCorp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace SeekerSoft.Core.WXCorp
{
    public class CorpManage
    {
        /// <summary>
        /// 根据用户微信号获取该用户实体
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="weixinid"></param>
        /// <returns></returns>
        public static WXUser GetUserByWeixinID(string accessToken, string weixinid)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

            WXUser user = null;

            //获取所有部门实体
            WXMsgResultDept WeixinDepart = (WXMsgResultDept)jsonSerializer.Deserialize(CorpManage.GetAllDepartmentInfo(accessToken), typeof(WXMsgResultDept));
            if (WeixinDepart != null)
            {
                List<WXOrg> weixinDeparts = WeixinDepart.department;
                foreach (WXOrg weixinDepart in weixinDeparts)   //循环所有部门
                {
                    //获取该部门下所有的用户
                    WXMsgResultUser WeixinUsers = (WXMsgResultUser)jsonSerializer.Deserialize(CorpManage.GetDepartUserDetails(accessToken, weixinDepart.id.Value), typeof(WXMsgResultUser));
                    List<WXUser> weixinUsers = WeixinUsers.userlist;
                    if (weixinUsers != null)
                    {
                        foreach (WXUser weixinUser in weixinUsers)    //循环用户，找出当前用户
                        {
                            if (weixinUser.weixinid != null)
                            {
                                if (weixinUser.weixinid.Equals(weixinid))
                                {
                                    user = weixinUser;  //发现当前用户
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return user;
        }
        /// <summary>
        /// 根据用户账号获取该用户实体
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static WXUser GetUserByAccount(string accessToken, string account)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

            WXUser user = null;

            //获取所有部门实体
            WXMsgResultDept WeixinDepart = (WXMsgResultDept)jsonSerializer.Deserialize(CorpManage.GetAllDepartmentInfo(accessToken), typeof(WXMsgResultDept));
            if (WeixinDepart != null)
            {
                List<WXOrg> weixinDeparts = WeixinDepart.department;
                foreach (WXOrg weixinDepart in weixinDeparts)   //循环所有部门
                {
                    //获取该部门下所有的用户
                    WXMsgResultUser WeixinUsers = (WXMsgResultUser)jsonSerializer.Deserialize(CorpManage.GetDepartUserDetails(accessToken, weixinDepart.id.Value), typeof(WXMsgResultUser));
                    List<WXUser> weixinUsers = WeixinUsers.userlist;
                    if (weixinUsers == null || weixinUsers.Count() == 0) continue;// 跳过没有人的部门

                    foreach (WXUser weixinUser in weixinUsers)    //循环用户，找出当前用户
                    {
                        if (weixinUser.userid.Equals(account))
                        {//发现当前用户
                            user = weixinUser; break;
                        }
                    }
                    if (user != null) break;
                }
            }

            return user;
        }

        /// <summary>
        /// 创建用户  返回json字符串{"errcode": 0, "errmsg": "created"}
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="user">WXUser实体</param>
        /// <returns></returns>
        public static WXMsgResult CreateUser(string accessToken, WXUser user)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/user/create?access_token={0}";
            var url = string.Format(urlFormat, accessToken);
            WXMsgResult _msg;
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string strjson = jsonSerializer.Serialize(user);
            byte[] bs = Encoding.UTF8.GetBytes(strjson);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WXMsgResult));
                    _msg = (WXMsgResult)serializer.ReadObject(ms);

                    //用户创建成功
                    if (_msg.errcode == 0)
                    {
                        //邀请关注
                        Invite(accessToken, user.userid);
                    }
                }
                return _msg;
            }
        }







        /// <summary>
        /// 邀请关注企业号
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="userid">用户的userid</param>
        /// <returns>结果</returns>
        public static string Invite(string accessToken, string userid)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/invite/send?access_token={0}";
            var url = string.Format(urlFormat, accessToken);

            string strjson = "{\"userid\":\"" + userid + "\",\"invite_tips\":\"请关注博士伦\"}";
            byte[] bs = Encoding.UTF8.GetBytes(strjson);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                return sb.ToString();
            }

        }




        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="user">WXUser实体</param>
        /// <returns></returns>
        public static WXMsgResult UpdateUser(string accessToken, WXUser user)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/user/update?access_token={0}";
            WXMsgResult _msg;
            var url = string.Format(urlFormat, accessToken);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string strjson = jsonSerializer.Serialize(user);
            byte[] bs = Encoding.UTF8.GetBytes(strjson);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();

                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WXMsgResult));
                    _msg = (WXMsgResult)serializer.ReadObject(ms);
                }
                return _msg;
            }
        }



        //删除用户
        /// <summary>
        /// 删除指定用户
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="userid">账号</param>
        /// <returns></returns>
        public static string DeleteUser(string accessToken, string userid)
        {

            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/user/delete?access_token={0}&userid={1}";
            var url = string.Format(urlFormat, accessToken.Trim(), userid.Trim());
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();

                return sb.ToString();
            }



        }


        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="users"></param>
        /// <returns></returns>
        public static string BatchdeleteUsers(string accessToken, List<string> users)
        {

            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/user/batchdelete?access_token={0}";
            var url = string.Format(urlFormat, accessToken);
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("{\"useridlist\":[");
            for (int i = 0; i < users.Count - 1; i++)
            {
                sb1.Append("\"" + users[0] + "\",");
            }
            sb1.Append("\"" + users[users.Count - 1] + "\"]}");
            byte[] bs = Encoding.UTF8.GetBytes(sb1.ToString());
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                return sb.ToString();
            }

        }


        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="id">部门id</param>
        /// <returns></returns>
        public static string GetDepartmentInfo(string accessToken, string id)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}&id={1}";
            var url = string.Format(urlFormat, accessToken.Trim(), id.Trim());
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();

                return sb.ToString();
            }
        }


        /// <summary>
        /// 获取所有部门信息
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="id">部门id</param>
        /// <returns></returns>
        public static string GetAllDepartmentInfo(string accessToken)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}";
            var url = string.Format(urlFormat, accessToken.Trim());
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();

                return sb.ToString();
            }
        }



        /// <summary>
        /// 查询微信的部门ID
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="id">部门名称</param>
        /// <returns></returns>
        public static string GetDepartmentID(string accessToken, string strDepartName)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}";
            var url = string.Format(urlFormat, accessToken.Trim());
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WXMsgResultDept));
                    WXMsgResultDept _msg = (WXMsgResultDept)serializer.ReadObject(ms);
                    if (_msg.errcode == 0)
                    {

                        var depart = from d in _msg.department
                                     where d.name == (strDepartName)
                                     select new { id = d.id };
                        return depart.First().id.ToString();

                    }

                }

                return "";
            }
        }


        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="Depart">部门实体</param>
        /// <returns></returns>
        public static WXMsgResultDeptCreate CreateDepartment(string accessToken, WXOrg Depart)
        {
            WXMsgResultDeptCreate _msg;
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/department/create?access_token={0}";
            var url = string.Format(urlFormat, accessToken);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string strjson = jsonSerializer.Serialize(Depart);
            byte[] bs = Encoding.UTF8.GetBytes(strjson);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WXMsgResultDeptCreate));
                    _msg = (WXMsgResultDeptCreate)serializer.ReadObject(ms);
                }
                return _msg;
            }
        }


        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WXMsgResult DeleteDepartment(string accessToken, int id)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/department/delete?access_token={0}&id={1}";
            WXMsgResult _msg;
            var url = string.Format(urlFormat, accessToken.Trim(), id);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WXMsgResult));
                    _msg = (WXMsgResult)serializer.ReadObject(ms);
                }
                return _msg;
            }
        }


        /// <summary>
        /// 更新部门
        /// </summary>
        /// <param name="accessToken">accessToken</param>
        /// <param name="Depart">部门实体</param>
        /// <returns></returns>
        public static string UpdateDepartment(string accessToken, WXOrg Depart)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/department/create?access_token={0}";
            var url = string.Format(urlFormat, accessToken);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string strjson = jsonSerializer.Serialize(Depart);
            byte[] bs = Encoding.UTF8.GetBytes(strjson);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                return sb.ToString();
            }
        }


        /// <summary>
        /// 获取部门下的成员(详情)
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetDepartUserDetails(string accessToken, int department_id)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={0}&department_id={1}&fetch_child=0&status=0";
            var url = string.Format(urlFormat, accessToken, department_id);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();

                return sb.ToString();

            }
        }

        /// <summary>
        /// 获取部门下的成员
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetDepartUsers(string accessToken, int department_id)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/user/simplelist?access_token={0}&department_id={1}&fetch_child=0&status=0";
            var url = string.Format(urlFormat, accessToken, department_id);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();

                return sb.ToString();

            }
        }


        public static AccessTokenResults Getaccesstoken(string accessToken, string code, string agentid)
        {
            var urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}&agentid={2}";
            var url = string.Format(urlFormat, accessToken, code, agentid);
            AccessTokenResults result = CorpManage.GetJson<AccessTokenResults>(url);
            return result;

        }

        /// <summary>
        /// 获取指定用户信息
        /// </summary>
        /// <param name="accessToken">accessToken</param>
        /// <param name="userid">用户账号</param>
        /// <returns></returns>
        public static WXUser GetUserInfo(string accessToken, string userid)
        {
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}";
            var url = string.Format(urlFormat, accessToken, userid);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WXUser));
                    return (WXUser)serializer.ReadObject(ms);
                }
            }

        }

        /// <summary>
        /// 发送消息。         
        /// 需要管理员对应用有使用权限，对收件人touser、toparty、totag有查看权限，否则本次调用失败。        
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string SendMessage(string accessToken, SendMessages data)
        {
            // 如果没有收信人，则向所有人发送次消息
            if (string.IsNullOrWhiteSpace(data.touser)
                && string.IsNullOrWhiteSpace(data.totag)
                && string.IsNullOrWhiteSpace(data.toparty))
            {
                data.touser = "@all";
            }

            SendMessages result = new SendMessages();
            string urlFormat = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";
            var url = string.Format(urlFormat, accessToken);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string strjson = jsonSerializer.Serialize(data);
            byte[] bs = Encoding.UTF8.GetBytes(strjson);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            //req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 
                Stream strm = wr.GetResponseStream();
                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);
                string line;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                return sb.ToString();
            }

        }


        public static T GetJson<T>(string url)
        {
            string returnText = RequestUtility.HttpGet(url);

            JavaScriptSerializer js = new JavaScriptSerializer();

            if (returnText.Contains("errcode"))
            {
                //可能发生错误
                WXMsgResult errorResult = js.Deserialize<WXMsgResult>(returnText);
                if (errorResult.errcode != ReturnCode.请求成功)
                {
                    //发生错误
                    throw new Exception(errorResult.errcode.ToString());
                }
            }

            T result = js.Deserialize<T>(returnText);

            return result;
        }
    }
}
