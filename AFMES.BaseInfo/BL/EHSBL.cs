using SSLPA.BaseInfo.DTO;
using SSLPA.DB;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.Config;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SSLPA.BaseInfo.BL
{
    public class EHSBL
    {
        public static string EHSPicPath = ConfigHelper.Get("EHSImgPath");   //EHS图片
        public static string TempPath = ConfigHelper.Get("TempPath");   //临时存放路径


        public static PagerResult<MEHS> Query(string keyword, PagerParams param)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"
SELECT  EHSID ,
        Name ,
        Pic ,
        Remark
FROM    dic.EHS
WHERE   IsDel = 0 ");
            if (!string.IsNullOrEmpty(keyword))
            {
                sql.AppendFormat(" AND Name LIKE '%{0}%'", keyword.Replace("'", "''"));
            }


            // 排序
            if (string.IsNullOrEmpty(param.sortField))
            {
                param.sortField = "EHSID";
                param.sortOrder = "DESC";
            }
            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MEHS>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        public static MEHS Get(int id)
        {
            var sql = @"
SELECT  EHSID ,
        Name ,
        Pic ,
        Remark
FROM    dic.EHS
WHERE   IsDel = 0 AND EHSID=@id";
            using (var db = DB.DBHelper.NewDB())
            {
                var model = DBHelper<MEHS>.ExecuteSqlToEntities(db, sql, new SqlParameter("id", id)).SingleOrDefault();
                return model;
            }
        }

        public static bool Add(UserInfo userinfo, MEHS model)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var m = new DB.EHS()
                {
                    Name = model.Name,
                    Pic = model.Pic,
                    IsDel = false,
                    Remark = model.Remark,
                    CreateTime = DateTime.Now,
                    CreateBy = userinfo.UserID
                };
                db.EHS.Add(m);
                db.SaveChanges();

                // 移动图片到对应文件夹
                string tempPhoto = HttpContext.Current.Request.MapPath(TempPath) + model.Pic;
                string newPhoto = HttpContext.Current.Request.MapPath(EHSPicPath) + model.Pic;
                if (!string.IsNullOrEmpty(model.Pic)
                    && File.Exists(tempPhoto))
                {
                    if (File.Exists(newPhoto)) File.Delete(newPhoto);
                    File.Move(tempPhoto, newPhoto);
                }
            }
            return true;
        }

        public static bool Save(UserInfo userInfo, MEHS model)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var service = db.EHS.SingleOrDefault(p => p.EHSID == model.EHSID);
                if (service == null) throw new Exception("找不到数据");

                // 移动头像
                if (service.Pic != model.Pic)
                {
                    // 移动图片到对应文件夹
                    string tempPhoto = HttpContext.Current.Request.MapPath(TempPath) + model.Pic;
                    string newPhoto = HttpContext.Current.Request.MapPath(EHSPicPath) + model.Pic;
                    string oldPhoto = HttpContext.Current.Request.MapPath(EHSPicPath) + service.Pic;
                    // pic为空，删除原头像
                    if (string.IsNullOrEmpty(model.Pic) && !string.IsNullOrEmpty(service.Pic))
                    {
                        if (File.Exists(oldPhoto)) File.Delete(oldPhoto);
                    }
                    else
                    {// 更新
                        if (File.Exists(oldPhoto)) File.Delete(oldPhoto);
                        if (File.Exists(newPhoto)) File.Delete(newPhoto);
                        if (File.Exists(tempPhoto)) File.Move(tempPhoto, newPhoto);
                    }
                }

                service.Name = model.Name;
                service.Pic = model.Pic;
                service.Remark = model.Remark;
                service.UpdateTime = DateTime.Now;
                service.ModifyBy = userInfo.UserID;
                db.SaveChanges();
            }

            return true;
        }

        public static bool Del(UserInfo userinfo, int[] ids)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var models = from dic in db.EHS
                             where ids.Contains(dic.EHSID)
                             select dic;
                foreach (var model in models)
                {
                    model.IsDel = true;
                }
                db.SaveChanges();
            }
            return true;
        }


        public static MEHS[] GetMachineEHS(int machineid)
        {
            if (machineid == 0)
                throw new Exception("必须提供设备ID");

            using (var db = DB.DBHelper.NewDB())
            {
                string sql = @"
SELECT  t2.EHSID ,
        t2.Name ,
        t2.Pic ,
        t2.Remark
FROM    dic.MachineEHS t1
        INNER JOIN dic.EHS t2 ON t2.EHSID = t1.EHSID AND t2.IsDel = 0
WHERE   t1.MachineID = @machineid";
                var result = DBHelper<MEHS>.ExecuteSqlToEntities(db, sql
                    , new System.Data.SqlClient.SqlParameter("machineid", machineid))
                    .ToArray();

                foreach (var li in result)
                {
                    li.Pic = EHSPicPath.Replace("~/", "") + li.Pic;
                }

                return result;
            }
        }

        public static bool SaveMachineEHS(int machineid, int[] ids)
        {
            if (machineid == 0)
                throw new Exception("必须提供设备ID");

            using (var db = DB.DBHelper.NewDB())
            {
                db.MachineEHS.RemoveRange(db.MachineEHS.Where(_ => _.MachineID == machineid));
                foreach (var li in ids)
                {
                    db.MachineEHS.Add(new MachineEHS()
                    {
                        EHSID = li,
                        MachineID = machineid
                    });
                }
                db.SaveChanges();
            }
            return true;
        }

    }
}
