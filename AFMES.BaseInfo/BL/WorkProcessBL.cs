using SSLPA.BaseInfo.DTO;
using SSLPA.DB;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.BL
{
    public class WorkProcessBL
    {
        public static PagerResult<MWorkProcess> WorkProcessList(UserInfo userInfo, QWorkProcess pagerParams)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT  t1.ProcID ,
        t1.Code ,
        t1.Name ,
        t1.ProcType ,
		ProcTypeName = t2.Name,
        t1.Remark ,
        t1.IsDel ,
        t1.CreateTime ,
        t1.CreateBy ,
        t1.UpdateTime ,t1.PDCode,
        t1.ModifyBy FROM dic.WorkProcess t1 
        LEFT JOIN  base.DicItem t2 ON t2.Code = t1.ProcType AND t2.DicCode = '1007'
        WHERE IsDel=0");

            // 非LPA管理员 只能查看本部门数据
            if (!userInfo.AuthList.Contains("2003"))
            {
                var pdCode = SeekerSoft.Base.BL.RoleBL.GetPdCode(userInfo.UserID);
                if (!string.IsNullOrEmpty(pdCode))
                {
                    sql.AppendFormat(" AND t1.PDCode = '{0}' ", pdCode);
                }
            }
            if (!string.IsNullOrEmpty(pagerParams.Keyword))
            {
                sql.AppendFormat(" AND (t1.Code LIKE '%{0}%' OR t1.Name LIKE '%{0}%')", pagerParams.Keyword.Replace("'", "''"));
            }

            // 排序
            if (string.IsNullOrEmpty(pagerParams.sortField)) pagerParams.sortField = "t1.Code";
            sql.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(pagerParams.sortField, pagerParams.sortOrder));

            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = pagerParams.pageSize;
            pageProcedureParams.pageIndex = pagerParams.pageIndex;
            pageProcedureParams.sql = sql.ToString();
            using (var db = DBHelper.NewDB())
            {
                PagerResult<MWorkProcess> data = DBHelper<MWorkProcess>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

        public static MWorkProcess WorkProcessGet(UserInfo userinfo, int id)
        {
            string sql = @"SELECT  t1.ProcID ,
		        t1.Code ,
		        t1.Name ,
                t1.ProcType,
		        t1.Remark ,
		        t1.IsDel ,
		        t1.CreateTime ,
		        t1.CreateBy ,
		        t1.UpdateTime ,t1.PDCode,
		        t1.ModifyBy FROM dic.WorkProcess t1 WHERE t1.ProcID=" + id;
            using (var db = DB.DBHelper.NewDB())
            {
                //排序获取当前页的数据
                var data = DBHelper<MWorkProcess>.ExecuteSqlToEntities(db, sql).SingleOrDefault();
                if (data == null)
                    throw new Exception("找不到指定的数据。");
                return data;
            }
        }

        public static bool WorkProcessAdd(UserInfo userinfo, MWorkProcess model)
        {
            using (var db = DBHelper.NewDB())
            {
                if (db.WorkProcess.Any(_ => !_.IsDel && _.Code == model.Code))
                    throw new Exception("该工序已存在!");

                //新增
                db.WorkProcess.Add(new WorkProcess()
                {
                    Name = model.Name,
                    Code = model.Code,
                    ProcType = "",
                    Remark = model.Remark,
                    IsDel = false,
                    CreateBy = userinfo.UserID,
                    CreateTime = DateTime.Now,
                    PDCode =model.PDCode
                });
                db.SaveChanges();
            }
            return true;
        }

        public static bool WorkProcessSave(UserInfo userinfo, MWorkProcess model)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.WorkProcess.Where(_ => _.ProcID == model.ProcID).SingleOrDefault();
                if (entity == null) throw new Exception("找不到指定的数据");
                if (db.WorkProcess.Any(_ => !_.IsDel && _.Code == model.Code && _.Code != entity.Code))
                    throw new Exception("该工序已存在!");
                entity.Name = model.Name;
                entity.Code = model.Code;
                entity.ProcType ="";
                entity.Remark = model.Remark;
                entity.ModifyBy = userinfo.UserID;
                entity.UpdateTime = DateTime.Now;
                entity.PDCode = model.PDCode;
                db.SaveChanges();
            }
            return true;
        }

        public static bool WorkProcessDel(UserInfo userinfo, int id)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.WorkProcess.Where(_ => _.ProcID == id).FirstOrDefault();
                if (m == null) throw new Exception("数据不存在");

                m.IsDel = true;
                m.ModifyBy = userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        public static MWorkProcess[] WorkProcessListByMaterialID(UserInfo userinfo, string type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT  ProcID ,
        Code ,
        Name ,
        ProcType,
        Remark ,
        ProcType
FROM    dic.WorkProcess
WHERE   IsDel = 0
        AND ProcType ='{0}'", type);

            using (var db = DBHelper.NewDB())
            {
                return DBHelper<MWorkProcess>.ExecuteSqlToEntities(db, sb.ToString()).ToArray();
            }
        }

        public static MWorkProcess[] QueryAll()
        {
            using (var db = DBHelper.NewDB())
            {
                return db.Database.SqlQuery<MWorkProcess>(@"
SELECT  ProcID ,
        Code ,
        Name  FROM dic.WorkProcess
		WHERE IsDel=0
").ToArray();
            }
        }

        /// <summary>
        /// 获取产品部门工序列表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="param"></param>
        /// <param name="pDCode"></param>
        /// <returns></returns>
        public static PagerResult<MWorkProcess> GetWorkPressList(UserInfo userInfo, QWorkProcess param, string pDCode)
        {
            StringBuilder sqlWhere = new StringBuilder();
            sqlWhere.AppendFormat(@"SELECT t1.ProcID ,t1.Code ,t1.Name  FROM dic.WorkProcess AS t1 WITH ( NOLOCK ) WHERE t1.IsDel=0 AND t1.PDCode='{0}'", pDCode);
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                sqlWhere.AppendFormat("  AND (t1.Name LIKE '%{0}%'  OR t1.Code LIKE '%{0}%' )", param.Keyword.Replace("'", "''"));
            }
            // 排序
            if (string.IsNullOrEmpty(param.sortField)) param.sortField = "ProcID";

            sqlWhere.AppendFormat(" ORDER BY {0} ", SqlPageProcedureParams.GetSortSQL(param.sortField, param.sortOrder));
            SqlPageProcedureParams pageProcedureParams = new SqlPageProcedureParams();
            pageProcedureParams.pageSize = param.pageSize;
            pageProcedureParams.pageIndex = param.pageIndex;
            pageProcedureParams.sql = sqlWhere.ToString();
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MWorkProcess>.ExecuteSqlPageProcedureToEntities(db, pageProcedureParams);
                return data;
            }
        }

    }
}