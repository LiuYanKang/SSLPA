using SeekerSoft.Base.DTO;
using SeekerSoft.Core.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeekerSoft.Core.ServiceModel;
using SeekerSoft.Base.DB;
using SeekerSoft.Core.DB;
using System.Data.SqlClient;

namespace SeekerSoft.Base.BL
{
    public class MenuBL
    {
        /// <summary>
        /// 增加根节点
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Save(UserInfo userinfo, MMenu model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Menu.Where(_ => _.MenuCode == model.MenuCode).SingleOrDefault();
                if (entity == null) throw new Exception("找不到指定的数据。");
                entity.Name = model.Name;
                entity.Icon = model.Icon;
                entity.Color = model.Color;
                entity.Url = model.Url;
                entity.Visible = model.Visible;
                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 增加子节点
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Add(UserInfo userinfo, MMenu model)
        {
            using (var db = DBHelper.NewDB())
            {
                if (db.Menu.Any(_ => _.MenuCode == model.MenuCode))
                    throw new Exception("代码" + model.MenuCode + "已存在");

                var m = new Menu()
                {
                    MenuCode = model.MenuCode,
                    Name = model.Name,
                    PCode = model.PCode,
                    Icon = model.Icon,
                    Color = model.Color,
                    Url = model.Url,
                    Visible = model.Visible
                };
                db.Menu.Add(m);
                db.SaveChanges();
            }
            return true;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userifo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool Delete(string MenuCode)
        {
            using (var db = DBHelper.NewDB())
            {
                db.Database.ExecuteSqlCommand(@"Delete base.Menu WHERE MenuCode=@MenuCode ", new SqlParameter("MenuCode", MenuCode));
            }
            return true;
        }

        /// <summary>
        /// 根据主键获取信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static MMenu GetByCode(string code)
        {
            using (var db = DBHelper.NewDB())
            {
                var model = db.Menu.Where(_ => _.MenuCode == code).OrderBy(_ => _.SN).Select(_ => new MMenu()
                {
                    MenuCode = _.MenuCode,
                    PCode = _.PCode,
                    Name = _.Name,
                    Icon = _.Icon,
                    Color = _.Color,
                    Url = _.Url,
                    Visible = _.Visible
                }).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");
                return model;
            }
        }

        public static MMenu[] QueryAll()
        {
            using (var db = DBHelper.NewDB())
            {
                return db.Menu.OrderBy(_ => _.MenuCode).OrderBy(_ => _.SN).Select(_ => new MMenu()
                {
                    MenuCode = _.MenuCode,
                    PCode = _.PCode,
                    Name = _.Name,
                    Icon = _.Icon,
                    Color = _.Color,
                    Url = _.Url,
                    Visible = _.Visible
                }).ToArray();
            }
        }
    }
}
