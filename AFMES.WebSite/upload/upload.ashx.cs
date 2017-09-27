using SeekerSoft.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SSLPA.WebSite.upload
{
    /// <summary>
    /// upload 的摘要说明
    /// </summary>
    public class upload : IHttpHandler
    {
        public static string TempPath = ConfigHelper.Get("TempPath");   //临时存放路径

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var type = context.Request.QueryString["type"];
            try
            {
                string serverPath = "";

                //找到目标文件对象
                HttpPostedFile uploadFile = context.Request.Files["file"];
                string fileExt = Path.GetExtension(uploadFile.FileName);
                string newShortFileName = Guid.NewGuid().ToString() + fileExt;
                // 如果有文件, 则保存到一个地址
                if (uploadFile.ContentLength > 0)
                {
                    switch (type)
                    {

                        default://临时图片存放路径
                            serverPath = context.Request.MapPath(TempPath);
                            break;
                    }

                    if (!Directory.Exists(serverPath))
                    {
                        Directory.CreateDirectory(serverPath);
                    }

                    uploadFile.SaveAs(Path.Combine(serverPath, newShortFileName));


                    context.Response.Write(newShortFileName);
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.ToString());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}