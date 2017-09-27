using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SeekerSoft.Core.Config;

namespace SSLPA.WebSite.BaseInfo
{
    /// <summary>
    /// upload 的摘要说明
    /// </summary>
    public class upload : IHttpHandler
    {
        public static string CustomerLogoPath = ConfigHelper.Get("CustomerLogoPath");        //客户Logo图标
        public static string TempFilePath = ConfigHelper.Get("TempPath");       //临时文件
        public static string LPAImgPath = ConfigHelper.Get("LPAImgPath");     //LPA

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
                        case "1": //客户Logo图标
                            serverPath = context.Request.MapPath(CustomerLogoPath);
                            break;
                        case "3":
                            serverPath = context.Request.MapPath(LPAImgPath);
                            break;
                        default://临时上传地址
                            serverPath = context.Request.MapPath(TempFilePath);
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