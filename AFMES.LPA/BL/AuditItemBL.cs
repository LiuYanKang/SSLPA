using SSLPA.DB;
using SSLPA.LPA.DTO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SSLPA.LPA.BL
{
    public class AuditItemBL
    {
        //获取审核项目
        public static MAuditItem AuditItemGet(int id)
        {
            string sql = @"SELECT  t1.ItemID ,
        t1.AuditType ,
        t1.ItemRegion ,
		ItemRegionName=t2.AreaName,
        t1.ItemType ,
		ItemTypeName=t3.Name,
        t1.Description ,
        t1.SN ,
        t1.MCode,
        t1.IsInputData
        FROM lpa.AuditItem t1
		LEFT JOIN lpa.Area t2 ON t1.ItemRegion=t2.AreaId 
		LEFT JOIN base.DicItem t3 ON t1.ItemType=t3.Code AND t3.DicCode='1026'
		WHERE t1.IsDel=0 AND t1.ItemID=@id ";

            using (var db = DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var model = DBHelper<MAuditItem>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", id)).SingleOrDefault();
                if (model == null)
                    throw new Exception("找不到指定的数据。");

                return model;
            }
        }

        //新增审核项目
        public static bool AuditItemAdd(UserInfo userinfo, MAuditItem model)
        {
            using (var db = DBHelper.NewDB())
            {
                //新增
                db.AuditItem.Add(new AuditItem()
                {
                    AuditType = model.AuditType,
                    ItemRegion = model.ItemRegion,
                    ItemType = model.ItemType,
                    Description = model.Description,
                    SN = 99,// 默认99，放在最后
                    IsDel = false,
                    CreateBy = userinfo.UserID,
                    CreateTime = DateTime.Now,
                    MCode = model.MCode,
                    IsInputData = model.IsInputData
                });
                db.SaveChanges();
            }
            return true;
        }

        //保存审核项目数据
        public static bool AuditItemSave(UserInfo userinfo, MAuditItem model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.AuditItem.Where(_ => _.ItemID == model.ItemID).SingleOrDefault();
                if (entity == null) throw new Exception("找不到指定的数据");

                entity.ItemRegion = model.ItemRegion;
                entity.ItemType = model.ItemType;
                entity.Description = model.Description;
                //entity.SN = model.SN;
                entity.ModifyBy = userinfo.UserID;
                entity.UpdateTime = DateTime.Now;
                entity.MCode = model.MCode;
                entity.IsInputData = model.IsInputData;
                db.SaveChanges();
            }
            return true;
        }

        //查询审核项目
        public static MAuditItem[] AuditItemList(UserInfo userInfo, QAuditItem param)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT  t1.ItemID ,
        t1.AuditType ,
        t1.ItemRegion ,
		ItemRegionName=t2.AreaName,
        t1.ItemType ,
		ItemTypeName=t3.Name,
        t1.Description ,
        t1.SN ,t4.PDCode,t1.MCode,MName=t5.Name
        FROM lpa.AuditItem t1
		LEFT JOIN lpa.Area t2 ON t1.ItemRegion=t2.AreaId 		
		LEFT JOIN dic.ProductDept AS t4 ON t4.PDCode=t2.PDCode
		LEFT JOIN base.DicItem t3 ON t1.ItemType=t3.Code AND t3.DicCode='1026'
		LEFT JOIN dic.Machine AS t5 ON t5.Code=t1.MCode 
		WHERE t1.IsDel=0 AND t5.Name!=''");
            sql.AppendFormat(" AND t1.AuditType = '{0}' ", param.AuditType);

            // 非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t4.PDCode = '{0}' ", pdCode);
                }
            }
            if (!string.IsNullOrEmpty(param.AuditArea))
            {
                sql.AppendFormat(" AND t1.ItemRegion = '{0}' ", param.AuditArea);
            }
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sql.AppendFormat("  AND (t2.Name LIKE '%{0}%' OR t1.Description LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
            }
            if (!string.IsNullOrEmpty(param.PDCode))
            {
                sql.AppendFormat(" AND t4.PDCode = '{0}' ", param.PDCode);
            }
           
            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = "ItemID";

            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));
            var sqlDic = new StringBuilder();
            sqlDic.Append(@"SELECT  t1.ItemID ,
        t1.AuditType ,
        t1.ItemRegion ,
		ItemRegionName=t2.AreaName,
        t1.ItemType ,
		ItemTypeName=t3.Name,
        t1.Description ,
        t1.SN ,t4.PDCode,t1.MCode,MName=t5.Name
        FROM lpa.AuditItem t1
		LEFT JOIN lpa.Area t2 ON t1.ItemRegion=t2.AreaId 		
		LEFT JOIN dic.ProductDept AS t4 ON t4.PDCode=t2.PDCode
		LEFT JOIN base.DicItem t3 ON t1.ItemType=t3.Code AND t3.DicCode='1026'
		LEFT JOIN base.DicItem AS t5 ON t1.MCode=t5.Code AND t5.DicCode = '1036'
		WHERE t1.IsDel=0 AND t5.Name!=''");
            sqlDic.AppendFormat(" AND t1.AuditType = '{0}' ", param.AuditType);

            if (!string.IsNullOrEmpty(param.AuditArea))
            {
                sqlDic.AppendFormat(" AND t1.ItemRegion = '{0}' ", param.AuditArea);
            }
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sqlDic.AppendFormat("  AND (t2.Name LIKE '%{0}%' OR t1.Description LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
            }
            if (!string.IsNullOrEmpty(param.PDCode))
            {
                sqlDic.AppendFormat(" AND t4.PDCode = '{0}' ", param.PDCode);
            }
            sqlDic.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MAuditItem>.ExecuteSqlToEntities(db, sql.ToString()).ToArray();
                var daDic = DBHelper<MAuditItem>.ExecuteSqlToEntities(db, sqlDic.ToString()).ToArray();
                return data.Union(daDic).ToArray();
            }
        }

        //删除审核项目
        public static bool AuditItemDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.AuditItem.Where(_ => _.ItemID == id).FirstOrDefault();
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = (int)userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }


        //拖拽之后 保存审核项目数据
        public static bool AuditItemSNSave(UserInfo userinfo, int[] idList)
        {
            using (var db = DBHelper.NewDB())
            {
                for (int i = 0; i < idList.Length; i++)
                {
                    int itemid = idList[i];
                    var entity = db.AuditItem.Where(_ => _.ItemID == itemid).SingleOrDefault();
                    if (entity != null)
                    {
                        entity.SN = i;
                        entity.ModifyBy = userinfo.UserID;
                        entity.UpdateTime = DateTime.Now;
                    }
                }

                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 获取相应审核项目的设备和其他
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MAuditMachine[] GetAuditMachine(int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var sql = string.Format(@"SELECT MCode=t1.Code,t1.Name  FROM base.DicItem t1 WHERE   t1.DicCode = '1036'");
                var dic = db.Database.SqlQuery<MAuditMachine>(sql).ToArray();
                var mach =db.Database.SqlQuery<MAuditMachine>(@"SELECT t1.Name,MCode=t1.Code FROM lpa.AreaMachineMap  AS  t3 WITH(NOLOCK)
                                                                LEFT JOIN dic.Machine AS t1 ON t3.MachineID=t1.MachineID
                                                                WHERE t3.AreaId=" + id);
                var mAuditMachines = mach.Union(dic);
                return mAuditMachines.ToArray();
            }
        }


    }
}
