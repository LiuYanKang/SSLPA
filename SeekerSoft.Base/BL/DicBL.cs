using SeekerSoft.Base.DB;
using SeekerSoft.Base.DTO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Base.BL
{
    public class DicBL
    {
        public static MDic[] DicAll(UserInfo userInfo, QDic pagerParams)
        {
            if (pagerParams == null) pagerParams = new QDic();

            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT  DicCode ,
        Name ,
        Remark FROM base.Dic
        WHERE 1=1");
            if (!string.IsNullOrEmpty(pagerParams.Keyword))
            {
                sql.AppendFormat(" AND (Name LIKE '%{0}%' OR DicCode LIKE '%{0}%')", pagerParams.Keyword.Replace("'", "''"));
            }

            // 排序
            if (string.IsNullOrEmpty(pagerParams.sortField)) pagerParams.sortField = "Name";

            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(pagerParams.sortField, pagerParams.sortOrder));

            using (var db = DBHelper.NewDB())
            {
                return DBHelper<MDic>.ExecuteSqlToEntities(db, sql.ToString()).ToArray();
            }
        }

        public static MDicItem[] DicItems(string dicCode)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var q = from dic in db.DicItem.AsNoTracking()
                        where dic.DicCode == dicCode
                        orderby dic.SN
                        select new MDicItem()
                        {
                            Code = dic.Code,
                            DicCode = dic.DicCode,
                            Name = dic.Name,
                            IsSys = dic.IsSys,
                            SN = dic.SN,
                            Remark = dic.Remark
                        };

                return q.ToArray();
            }
        }

        public static bool SaveDicItem(UserInfo userinfo, MDicItem model)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var m = db.DicItem.Where(_ => _.Code == model.Code && _.DicCode == model.DicCode).FirstOrDefault();
                if (m == null)
                {// 新增
                    m = new DicItem()
                    {
                        Code = model.Code,
                        DicCode = model.DicCode,
                        Name = model.Name,
                        IsSys = false,
                        SN = model.SN,
                        Remark = model.Remark
                    };
                    db.DicItem.Add(m);
                }
                else
                {// 更新
                    m.Name = model.Name;
                    m.Remark = model.Remark;
                    m.SN = model.SN;
                }
                db.SaveChanges();
            }

            return true;
        }
        public static bool DelDicItem(UserInfo userinfo, string dicCode, string code)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var m = db.DicItem.Where(_ => _.Code == code && _.DicCode == dicCode).FirstOrDefault();
                if (m == null) throw new Exception("数据不存在。");
                if (m.IsSys) throw new Exception("系统数据不允许被删除。");

                db.DicItem.Remove(m);
                db.SaveChanges();
            }

            return true;
        }

        public static string GetName(string dicCode, string code)
        {
            using (var db = DB.DBHelper.NewDB())
            {
                var q = from dic in db.DicItem.AsNoTracking()
                        where dic.DicCode == dicCode && dic.Code == code
                        select dic.Name;

                return q.FirstOrDefault();
            }
        }
    }
}
