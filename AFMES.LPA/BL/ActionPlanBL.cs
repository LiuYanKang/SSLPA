using SSLPA.DB;
using SSLPA.LPA.DTO;
using SeekerSoft.Core.Json;
using SeekerSoft.Core.IO;
using SeekerSoft.Core.Auth;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.ServiceModel;
using SeekerSoft.Core.Config;
using SeekerSoft.Base.BL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace SSLPA.LPA.BL
{
    public class ActionPlanBL
    {
        public static string LPAImgPath = ConfigHelper.Get("LPAImgPath");   //LPA问题图片
        public static string TempPath = ConfigHelper.Get("TempPath");   //临时存放路径


        public static MActionPlan GetPlanTobe(UserInfo userinfo)
        {
            string sql = @"SELECT  t1.PlanID ,
        t1.AuditType,
        AuditTypeName=t2.Name ,
        StartPlanDate=CONVERT(varchar(100), t1.StartPlanDate, 120),
        EndPlanDate=CONVERT(varchar(100), t1.EndPlanDate, 120)
        FROM lpa.ActionPlan t1,base.DicItem t2
		WHERE t1.AuditType=t2.Code AND t2.DicCode='1024'
		AND t1.IsDel=0 AND t1.IsComplete=0 AND
        t1.StartPlanDate<=CONVERT(VARCHAR(10),getdate(),120) AND t1.EndPlanDate>=CONVERT(VARCHAR(10),getdate(),120) AND t1.EmpID=@id";

            using (var db = DBHelper.NewDB())
            {
                var model = DBHelper<MActionPlan>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", userinfo.EmpID)).FirstOrDefault();
                return model;
            }
        }

        public static MActionPlan[] GetnPlanListTobe(UserInfo userinfo)
        {
            string sql = @"SELECT  top(10) StartPlanDate=CONVERT(varchar(100), t1.StartPlanDate, 23),
        EndPlanDate=CONVERT(varchar(100), t1.EndPlanDate, 23)
        FROM lpa.ActionPlan t1
		WHERE t1.IsDel=0 AND t1.IsComplete=0 AND
        t1.StartPlanDate>CONVERT(VARCHAR(10),getdate(),23) AND t1.EmpID=@id ORDER BY StartPlanDate asc";

            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MActionPlan>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", userinfo.EmpID)).ToArray();
                return data;
            }
        }

        public static bool ActionAdd(UserInfo userinfo, MAction model)
        {
            using (var db = DBHelper.NewDB())
            {
                //新增
                db.Action.Add(new SSLPA.DB.Action()
                {
                    PlanID = model.PlanID,
                    AuditDate = DateTime.Parse(model.AuditDate),
                    ProductName = model.ProductName,
                    AuditArea = model.AuditArea,
                    State = model.State,
                    IsDel = false,
                    CreateBy = userinfo.EmpID.HasValue ? userinfo.EmpID.Value : userinfo.UserID,
                    CreateTime = DateTime.Now,
                });
                db.SaveChanges();
            }
            return true;
        }

        public static MAuditGroup[] GetAuditItemList(UserInfo userinfo, int planid, string area)
        {
            string sql = @"SELECT 
        t5.ActionID,
        t2.ItemID,
        t2.AuditType ,
        t2.ItemRegion ,
        ItemRegionName=t3.AreaName,
        t2.ItemType ,
        ItemTypeName=t4.Name,
        t2.Description ,
        t6.Result,
        t2.IsInputData,
        t6.EnterNum,MName=t7.Name,MCode=t7.Code 
        FROM lpa.ActionPlan t1
        INNER JOIN lpa.AuditItem t2 ON t2.AuditType = t1.AuditType AND t2.IsDel=0
        INNER JOIN dic.Machine AS t7 ON t2.MCode=t7.Code
        LEFT JOIN lpa.Area t3 ON t2.ItemRegion=t3.AreaId 
        LEFT JOIN base.DicItem t4 ON t2.ItemType=t4.Code AND t4.DicCode='1026'
        LEFT JOIN lpa.Action t5 ON t5.PlanID = t1.PlanID AND t1.IsDel=0
        LEFT JOIN lpa.LPAActionResult t6 ON t6.ItemID = t2.ItemID AND t6.ActionID = t5.ActionID
        WHERE t1.PlanID=@planid AND t2.ItemRegion=@area ORDER BY t2.SN";

            var sqlDic = @"SELECT 
        t5.ActionID,
        t2.ItemID,
        t2.AuditType ,
        t2.ItemRegion ,
        ItemRegionName=t3.AreaName,
        t2.ItemType ,
        ItemTypeName=t4.Name,
        t2.Description ,
        t6.Result,
        t6.EnterNum,
        t2.IsInputData,
		MName=t7.Name,MCode=t7.Code
        FROM lpa.ActionPlan t1
        INNER JOIN lpa.AuditItem t2 ON t2.AuditType = t1.AuditType AND t2.IsDel=0
		 INNER JOIN base.DicItem AS t7 ON t2.MCode=t7.Code AND t7.DicCode = '1036'
        LEFT JOIN lpa.Area t3 ON t2.ItemRegion=t3.AreaId 
        LEFT JOIN base.DicItem t4 ON t2.ItemType=t4.Code AND t4.DicCode='1026'
        LEFT JOIN lpa.Action t5 ON t5.PlanID = t1.PlanID AND t1.IsDel=0
        LEFT JOIN lpa.LPAActionResult t6 ON t6.ItemID = t2.ItemID AND t6.ActionID = t5.ActionID
        WHERE t1.PlanID=@planid AND   t2.ItemRegion=@area ORDER BY t2.SN";

            using (var db = DBHelper.NewDB())
            {
                MAction actiondata = GetActionByPlanId(userinfo, planid);
                var data = DBHelper<MAuditItem>.ExecuteSqlToEntities(db, sql, new SqlParameter[] { new SqlParameter("@area", area), new SqlParameter("@planid", planid) }).ToArray();

                MAuditGroup[] dd = data.GroupBy(_ => new { _.MCode, _.MName }).Select(k =>
                  {
                      var gp = new MAuditGroup() { Code = k.Key.MCode, Name = k.Key.MName };
                      gp.Items = data.Where(i => i.MCode == k.Key.MCode).ToArray();
                      return gp;
                  }).ToArray();
                var daDic = DBHelper<MAuditItem>.ExecuteSqlToEntities(db, sqlDic, new SqlParameter[] { new SqlParameter("@area", area), new SqlParameter("@planid", planid) }).ToArray();
                MAuditGroup[] did = daDic.GroupBy(_ => new { _.MCode, _.MName }).Select(k =>
                {
                    var gp = new MAuditGroup() { Code = k.Key.MCode, Name = k.Key.MName };
                    gp.Items = daDic.Where(i => i.MCode == k.Key.MCode).ToArray();
                    return gp;
                }).ToArray();
                var mAuditGroups = dd.Union(did).ToArray();
                return mAuditGroups;
            }
        }

        public static MAction GetActionByPlanId(UserInfo userinfo, int planid)
        {
            string sql = @"SELECT  t1.ActionID,
        AuditDate=CONVERT(varchar(100), t1.AuditDate, 120),
        t1.PlanID,
        t1.ProductName,
        t1.AuditArea,
        t1.State,
        StateName=t2.Name
        FROM lpa.Action t1,base.DicItem t2
		WHERE t1.IsDel=0 AND t1.State=t2.Code AND t2.DicCode='1027' AND
        t1.PlanID=@id";

            using (var db = DBHelper.NewDB())
            {
                var model = DBHelper<MAction>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", planid)).SingleOrDefault();
                if (model == null)
                {
                    model = new MAction()
                    {
                        PlanID = planid,
                        AuditDate = DateTime.Now.ToString(),
                        State = "1",
                    };
                    if (ActionAdd(userinfo, model))
                    {
                        model = DBHelper<MAction>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", planid)).SingleOrDefault();
                    }
                }
                return model;
            }
        }

        public static MActionResult[] GetActionResultList(int actionid)
        {
            string sql = @"SELECT  t1.ActionID,
        t1.ItemID,
        t1.Result
        FROM lpa.LPAActionResult t1
		WHERE t1.ActionID=@id";

            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MActionResult>.ExecuteSqlToEntities(db, sql, new SqlParameter("@id", actionid)).ToArray();
                return data;
            }
        }

        public static bool ActionResultSave(List<MActionResult> models)
        {
            using (var db = DBHelper.NewDB())
            {
                foreach (MActionResult model in models)
                {
                    var entity = db.LPAActionResult.Where(_ => _.ItemID == model.ItemID && _.ActionID == model.ActionID).SingleOrDefault();
                    if (entity == null)
                    {
                        db.LPAActionResult.Add(new LPAActionResult()
                        {
                            ActionID = model.ActionID,
                            ItemID = model.ItemID,
                            Result = model.Result,
                            EnterNum = model.EnterNum
                        });
                    }
                    else
                    {
                        entity.Result = model.Result;
                        entity.EnterNum = model.EnterNum;
                    }
                }
                db.SaveChanges();
            }
            return true;
        }

        public static bool ActionStateChange(int actionid, string state)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Action.Where(_ => _.ActionID == actionid).SingleOrDefault();
                var problems = db.Problem.Where(_ => _.ActionID == actionid).ToArray();
                if (entity == null)
                {
                    return false;
                }
                foreach (var p in problems)
                {
                    p.State = "1";
                }
                entity.State = state;
                db.SaveChanges();
            }
            return true;
        }

        public static bool ActionTimeUpdate(int actionid)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.Action.Where(_ => _.ActionID == actionid).SingleOrDefault();
                if (entity == null)
                {
                    return false;
                }
                entity.AuditDate = DateTime.Now;
                var plan = db.ActionPlan.Where(_ => _.PlanID == entity.PlanID).SingleOrDefault();
                plan.ActionTime = DateTime.Now;
                plan.IsComplete = true;
                db.SaveChanges();
            }
            return true;
        }

        public static bool IsComplete(int actionid)
        {
            string sql = @"
SELECT CASE WHEN COUNT(1)=COUNT(Result) THEN 1 ELSE 0 END
FROM lpa.Action t1 
INNER JOIN lpa.ActionPlan t2 ON t1.PlanID=t2.PlanID
INNER JOIN lpa.EmpAuditArea t3 ON t3.EmpID = t2.EmpID
INNER JOIN lpa.Area are ON are.AreaId=t3.AuditArea
INNER JOIN lpa.PlanAreaMap  pam ON are.AreaId=pam.AreaId AND  pam.PlanID=t1.PlanID
INNER JOIN lpa.AuditItem t4 ON t4.AuditType = t2.AuditType AND t4.ItemRegion=t3.AuditArea AND t4.IsDel=0
LEFT JOIN lpa.LPAActionResult t5 ON t5.ActionID = t1.ActionID AND t5.ItemID = t4.ItemID
WHERE t1.ActionID=@actionid";
            using (var db = DBHelper.NewDB())
            {
                int result = db.Database.SqlQuery<int>(sql, new SqlParameter("actionid", actionid)).FirstOrDefault();
                if (result == 1)
                    return true;
                else
                    return false;
            }
        }

        public static int SaveActionResultList(int actionid, int state, List<MActionResult> models)
        {
            // 长度为0的，直接做成功返回
            if (models == null || models.Count == 0)
                return 2;
            ActionResultSave(models);
            if (state == 1)
            {
                if (IsComplete(actionid))
                {
                    if (ActionStateChange(actionid, "2"))
                    {
                        var isCom=ActionTimeUpdate(actionid);
                        if (isCom)
                        {
                            SendProblemEmail(actionid);
                        }
                        return 1;
                    }
                }
                else
                {
                    return 2;
                }
            }
            else if (state == 0 && models.Count > 0)
            {
                if (ActionStateChange(actionid, "1"))
                {
                    return 1;
                }
            }
            return 0;
            //ProductSave(actionid);
        }

        public static MRegionTypeResponse GetRegionTypeResponseEmp(UserInfo userinfo, int planid)
        {
            string sql1 = @"SELECT  t1.EmpID ,
		EmpName=t2.Name,
        t1.Position ,
        t1.SuperiorID ,
		SuperiorName=t3.Name,
        t1.IsResponsible  FROM lpa.Employee t1
		LEFT JOIN base.Employee t2 ON t2.EmpID = t1.EmpID
		LEFT JOIN base.Employee t3 ON t1.SuperiorID=t3.EmpID
		WHERE t1.IsDel=0";

            using (var db = DBHelper.NewDB())
            {
                MRegionTypeResponse model = new MRegionTypeResponse();
                var emps = DBHelper<MEmployee>.ExecuteSqlToEntities(db, sql1).ToArray();
                model.Emps = emps;
                model.ItemsRegion = EmployeeBL.GetAuditArea(planid);
                model.ItemsType = DicBL.DicItems("1026");
                return model;
            }
        }

        public static bool ActionResultChange(int actionid, int itemid, int value)
        {
            using (var db = DBHelper.NewDB())
            {
                var entity = db.LPAActionResult.Where(_ => _.ItemID == itemid && _.ActionID == actionid).SingleOrDefault();
                if (entity == null)
                {
                    return false;
                }
                entity.Result = value;
                db.SaveChanges();
            }
            return true;
        }

        public static bool AddProblem(UserInfo userinfo, MProblem model)
        {
            using (var db = DBHelper.NewDB())
            {
                DateTime time = DateTime.Now;
                SSLPA.DB.Problem problem = new Problem()
                {
                    ActionID = model.ActionID,
                    ItemID = model.ItemID,
                    ProblemRegion = model.ProblemRegion.Value,
                    MachineID = model.MachineID,
                    ProblemType = model.ProblemType,
                    Responsible = model.Responsible.Value ,
                    ProblemDesc = model.ProblemDesc,
                    SubmitDate = time,
                    State = "0",
                    Progress = 0,
                    CreateTime = time,
                    CreateBy = userinfo.EmpID ?? 0,
                    ImproveAdvice = model.ImproveAdvice
                };
                db.Problem.Add(problem);
                db.SaveChanges();                
                //int probid = problem.ProbID.HasValue ? problem.ProbID.Value : 0;
                if (model.Images != null && model.Images.Length > 0)
                {
                    model.BeforeProbPicList = model.Images;
                }
                if (model.BeforeProbPicList != null && model.BeforeProbPicList.Length > 0)
                {
                    foreach (var pic in model.BeforeProbPicList)
                    {
                        File.Move(HttpContext.Current.Request.MapPath(TempPath) + pic.FileName
                            , HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName);

                        db.ProblemPic.Add(new ProblemPic()
                        {
                            ProbID = problem.ProbID,
                            PicType = "1",
                            FileName = pic.FileName,
                            CreateTime = DateTime.Now,
                            CreateBy = userinfo.UserID
                        });
                    }
                }
                db.SaveChanges();

            }
            return true;
        }


        public static bool SubmitNcProblem(UserInfo userinfo, MProblem model)
        {
            using (var db = DBHelper.NewDB())
            {
                DateTime time = DateTime.Now;
                SSLPA.DB.Problem problem = new Problem()
                {
                    ActionID = model.ActionID,
                    ItemID = model.ItemID,
                    ProblemRegion = model.ProblemRegion.Value,
                    MachineID = model.MachineID,
                    ProblemType = model.ProblemType,
                    Responsible = model.Responsible.Value,
                    ProblemDesc = model.ProblemDesc,
                    SubmitDate = time,
                    State = "2",
                    Progress = 100,
                    CreateTime = time,
                    CreateBy = userinfo.EmpID ?? 0
                };
                db.Problem.Add(problem);
                db.SaveChanges();
                //int probid = problem.ProbID.HasValue ? problem.ProbID.Value : 0;
                if (model.Images != null && model.Images.Length > 0)
                {
                    model.BeforeProbPicList = model.Images;
                }
                if (model.BeforeProbPicList != null && model.BeforeProbPicList.Length > 0)
                {
                    foreach (var pic in model.BeforeProbPicList)
                    {
                        File.Move(HttpContext.Current.Request.MapPath(TempPath) + pic.FileName
                            , HttpContext.Current.Request.MapPath(LPAImgPath) + pic.FileName);

                        db.ProblemPic.Add(new ProblemPic()
                        {
                            ProbID = problem.ProbID,
                            PicType = "1",
                            FileName = pic.FileName,
                            CreateTime = DateTime.Now,
                            CreateBy = userinfo.UserID
                        });
                    }
                }
                db.SaveChanges();

            }
            return true;
        }


        public static MProblem[] GetItemDetail(int actionid, int itemid)
        {
            string sql = @"SELECT  t1.ProbID,
        t1.ProblemDesc,
        t1.Responsible
        FROM lpa.Problem t1
		WHERE t1.ActionID=@actionid AND t1.IsDel=0 AND t1.ItemID=@itemid";

            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql, new SqlParameter[] { new SqlParameter("@actionid", actionid), new SqlParameter("@itemid", itemid) }).ToArray();
                return data;
            }
        }

        public static bool ProblemDel(UserInfo userinfo, int probid)
        {
            using (var db = DBHelper.NewDB())
            {
                var m = db.Problem.Where(_ => _.ProbID == probid).FirstOrDefault();
                if (m == null) return false;

                m.IsDel = true;
                m.ModifyBy = (int)userinfo.UserID;
                m.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        public static MProblem[] GetProbList(int actionid, string area)
        {
            string sql = @"SELECT  t1.ProbID,
        t1.ProblemDesc,
        t1.Responsible,
        ItemName=t2.Description
        FROM lpa.Problem t1,lpa.AuditItem t2
		WHERE t1.ActionID=@actionid AND t1.ItemID=t2.ItemID AND t1.IsDel=0 AND t1.ProblemRegion=@area";
            string sql2 = @"SELECT  t1.ProbID,
        t1.ProblemDesc,
        t1.Responsible,
        ItemName=''
        FROM lpa.Problem t1
		WHERE t1.ActionID=@actionid AND t1.ItemID is NULL AND t1.IsDel=0 AND t1.ProblemRegion=@area";
            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql, new SqlParameter[] { new SqlParameter("@area", area), new SqlParameter("@actionid", actionid) }).ToArray();
                var data2 = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql2, new SqlParameter[] { new SqlParameter("@area", area), new SqlParameter("@actionid", actionid) }).ToArray();
                MProblem[] models = new MProblem[data.Length + data2.Length];
                data.CopyTo(models, 0);
                data2.CopyTo(models, data.Length);
                return Sort(models);
            }
        }

        public static MProblem[] Sort(MProblem[] models)
        {
            for (int i = 0; i < models.Length - 1; i++)
            {
                for (int j = 0; j < models.Length - i - 1; j++)
                {
                    if (models[j].ProbID > models[j + 1].ProbID)
                    {
                        MProblem temp = models[j];
                        models[j] = models[j + 1];
                        models[j + 1] = temp;
                    }
                }
            }
            return models;
        }

        
        //public static bool CheckExpired()
        public static void CheckExpired()
        {
            string sql = @"SELECT 	
		CONVERT(varchar(100), t1.StartPlanDate, 120) AS StartDate,
        CONVERT(varchar(100), t1.EndPlanDate, 120) AS EndDate,
        t1.EmpID,
        t2.Name,
        t2.EMail AS Email,
		t1.AuditType,
		AuditTypeName=t5.Name,
        t4.Name AS SuperName,
		t4.EMail AS SuperEmail,
		DATEDIFF(DAY, GETDATE(),t1.EndPlanDate) AS Expired,
		t1.BanCi,
		BanCiName=t6.Name,
		AudtiAreaMes=STUFF((SELECT ','+ are.AreaName FROM lpa.PlanAreaMap pam INNER JOIN lpa.Area are ON pam.AreaId=are.AreaId WHERE pam.PlanID=t1.PlanID  FOR XML PATH('')),1,1,'')
        FROM lpa.ActionPlan t1
		LEFT JOIN base.Employee t2 ON t2.EmpID=t1.EmpID
		LEFT JOIN base.Department t3 ON t2.DeptID=t3.DeptID
		LEFT JOIN base.Employee t4 ON t4.EmpID=t3.ManagerId 
		LEFT JOIN base.DicItem t5 ON t1.AuditType=t5.Code AND t5.DicCode='1024' 
		LEFT JOIN base.DicItem t6 ON t1.BanCi=t6.Code AND t6.DicCode='1030' 
        WHERE t1.IsDel=0 AND t1.IsComplete=0 AND( DATEDIFF(DAY, GETDATE(),t1.EndPlanDate)=1 OR DATEDIFF(DAY, GETDATE(),t1.EndPlanDate)=0) ";

            string sql2 = @"
        SELECT  
        t4.EmpID ,
        ProblemRegionName=t2.AreaName,
        ProblemTypeName=t8.Name,
        t3.ProblemDesc ,
        CONVERT(VARCHAR(100), t3.CreateTime, 111) AS StartDate,
        t4.Name ,
        t4.EMail AS Email ,
        t5.Name AS SuperName ,
        t5.EMail AS SuperEmail ,
        CONVERT(VARCHAR(100), t3.PlanEndDate, 120) AS EndDate ,
        DATEDIFF(DAY, GETDATE(), t3.PlanEndDate) AS Expired,
    	CreateByName=t6.Name
FROM    lpa.Problem t3
        LEFT JOIN lpa.Area t2 ON t3.ProblemRegion=t2.AreaId 
        LEFT JOIN base.DicItem t8 ON t3.ProblemType=t8.Code AND t8.DicCode='1026'
        LEFT JOIN base.Employee t4 ON t4.EmpID = t3.Responsible
        LEFT JOIN base.Department t6 ON t4.DeptID = t6.DeptID
        LEFT JOIN base.Employee t5 ON t5.EmpID = t6.ManagerId
	    LEFT JOIN base.Employee t7 ON t3.CreateBy=t7.EmpID
        WHERE   t3.IsDel = 0 AND t3.State = 1
order by t4.Name
";
            using (var db = DB.DBHelper.NewDB())
            {
                var data1 = DBHelper<MActionPlan2>.ExecuteSqlToEntities(db, sql).ToArray();

                var data2 = DBHelper<MProblem>.ExecuteSqlToEntities(db, sql2).ToArray();

                MMailList list1 = new MMailList();
                MMailList list2 = new MMailList();

               
                MPlanGroup[] dd = data1.GroupBy(_ => new { _.Email, _.Name, _.SuperName, _.SuperEmail }).Select(k => {
                    var gp = new MPlanGroup() { Name = k.Key.Name, Email = k.Key.Email, SuperName = k.Key.SuperName, SuperEmail = k.Key.SuperEmail };
                    gp.Items = data1.Where(i => i.Email == k.Key.Email && i.Name == k.Key.Name).ToArray();
                    return gp;
                }).ToArray();

                MPlanGroup[] ee = data1.GroupBy(_ => new { _.SuperName, _.SuperEmail }).Select(k => {
                    var gp = new MPlanGroup() { SuperName = k.Key.SuperName, SuperEmail = k.Key.SuperEmail };
                    gp.Items = data1.Where(i => i.SuperName == k.Key.SuperName && i.SuperName == k.Key.SuperName).ToArray();
                    return gp;
                }).ToArray();

                MProGroup[] ff = data2.GroupBy(_ => new { _.Email, _.Name, _.SuperEmail, _.SuperName }).Select(k => {
                    var gp = new MProGroup() { Name = k.Key.Name, Email = k.Key.Email, SuperName = k.Key.SuperName, SuperEmail = k.Key.SuperEmail };
                    gp.Items = data2.Where(i => i.Email == k.Key.Email && i.Name == k.Key.Name).ToArray();
                    return gp;
                }).ToArray();


                MProGroup[] gg = data2.GroupBy(_ => new { _.SuperEmail, _.SuperName }).Select(k => {
                    var gp = new MProGroup() { SuperName = k.Key.SuperName, SuperEmail = k.Key.SuperEmail };
                    gp.Items = data2.Where(i => i.SuperEmail == k.Key.SuperEmail && i.SuperName == k.Key.SuperName).ToArray();
                    return gp;
                }).ToArray();


                //给员工发邮件
                //执行计划和问题一起发
                foreach (var plan in dd)
                {
                    var dtList = ff.Where(x => x.Name == plan.Name && x.Email == plan.Email).ToArray();

                    list1.DataPlan = plan.Items;

                    if (dtList != null && dtList.Length > 0)
                    {
                        list1.DataProblem = dtList[0].Items;
                        string title = string.Format("尊敬的{0}，LPA系统里你的{1}个审核计划尚未完成，有{2}个问题尚未完成", plan.Name, plan.Items.Length, dtList[0].Items.Length);
                        EmailProvider.SendEmail(plan.Email, "LPA Reminding Message", MailListBL.GetMailList(list1, title, true, true), true);

                    }
                    else
                    {
                        string title = string.Format("尊敬的{0}，LPA系统里你的{1}个审核计划尚未完成", plan.Name, plan.Items.Length);
                        EmailProvider.SendEmail(plan.Email, "LPA Reminding Message", MailListBL.GetMailList(list1, title, true, false), true);
                    }
                    ff = ff.Where(x => x.Name != plan.Name).ToArray();
                }

                //发剩余的问题
                foreach (var t in ff)
                {
                    string title = string.Format("尊敬的{0}，LPA系统里你有{1}个问题尚未完成", t.Name, t.Items.Length);
                    list1.DataProblem = t.Items;
                    EmailProvider.SendEmail(t.Email, "LPA Reminding Message", MailListBL.GetMailList(list1, title, false, true), true);

                }

                //给领导发邮件
                //判断要不要给领导发计划，t.Ex = 0时，给领导发执行计划
                foreach (var t in ee)
                {
                    for (int i = 0; i < t.Items.Length; i++)
                    {
                        if (t.Items[i].Expired == 0)
                        {
                            t.Ex = 0;
                            break;
                        }
                        else
                        {
                            t.Ex = 1;
                        }
                    }
                }

                ee = ee.Where(x => x.Ex == 0).ToArray();

                //给同一领导发 执行计划和问题
                
                foreach (var plan in ee)
                {
                   
                    var dtList = gg.Where(x => x.SuperName == plan.SuperName && x.SuperEmail == plan.SuperEmail).ToArray();

                    list2.DataPlan = plan.Items;
                    

                    if (dtList != null && dtList.Length > 0)
                    {
                        list2.DataProblem = dtList[0].Items;
                        string title = string.Format("尊敬的{0}，LPA系统里你的下属有{1}个审核计划尚未完成，有{2}个问题尚未完成", plan.SuperName, plan.Items.Length, dtList[0].Items.Length);        
                        EmailProvider.SendEmail(plan.SuperEmail, "LPA Reminding Message", MailListBL.GetMailList(list2, title, true, true), true);

                    }
                    else
                    {
                        string title = string.Format("尊敬的{0}，LPA系统里你的下属有{1}个审核计划尚未完成", plan.SuperName, plan.Items.Length);
                        EmailProvider.SendEmail(plan.SuperEmail, "LPA Reminding Message", MailListBL.GetMailList(list2, title, true, false), true);
                    }

                    //ee.ToList().RemoveAll(a=>a.SuperEmail.Contains(plan.SuperEmail));
                    gg = gg.Where(x => x.SuperName != plan.SuperName).ToArray();
                }

                
                MMailList list3 = new MMailList();                
                //给领导发剩余问题
                foreach (var pro in gg)
                {
                    if (string.IsNullOrEmpty(pro.SuperEmail))
                        continue;
                    list3.DataProblem = pro.Items;
                    string title = string.Format("尊敬的{0}，LPA系统里你的下属有{1}个问题尚未完成", pro.SuperName, pro.Items.Length);
                    EmailProvider.SendEmail(pro.SuperEmail, "LPA Reminding Message", MailListBL.GetMailList(list3, title, false, true), true);
                }
            }
            
              
        //return true;
        }

        /// <summary>
        /// 发送问题责任人邮件
        /// </summary>
        /// <param name="actionid"></param>
        /// <returns></returns>
        public static bool SendProblemEmail(int actionid)
        {
            using (var db = DBHelper.NewDB())
            {
                var sql = string.Format(@"SELECT COUNT(1) Total ,t2.EMail AS Email,t2.Name FROM lpa.Problem AS t1 WITH(NOLOCK)
                                          LEFT JOIN base.Employee t2 ON t2.EmpID = t1.Responsible
                                          WHERE t1.IsDel=0 AND t1.ActionID={0} 
                                          GROUP BY 
                                          t2.EMail,t2.Name", actionid);
                var problist = DBHelper<MExpired>.ExecuteSqlToEntities(db, sql).ToArray();
                foreach (var prob in problist)
                {
                    var mes = EmailProvider.SendEmail(prob.Email, "LPA New Problem", string.Format(email_tmpl_5, prob.Name, DateTime.Now.Date.ToShortDateString(), prob.Total, SeekerSoft.Core.Config.ConfigHelper.Get("LoginUrl")), true);
                }
            }
            return true;
        }

        // 本人待执行计划
        const string email_tmpl_1 = @"<div style='font-family:Arial;font-size:10pt'><div>Dear {0},</div><div><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div><br></div><div>There is an LPA audit plan NOT FINISHED in LPA system which deadline is <b>{2}</b> ,&nbsp;please check and close it/them as soon as possible.</div><br></blockquote></div><div><br></div><div>Thank you!<br></div><div><br></div><div>尊敬的{0}，</div><br><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div>LPA系统里你有一个审核计划尚未完成，将于<b>{2}</b>截止，请在规定时间内完成该计划。</div><br></blockquote><br><div>谢谢！</div><div><br></div><div>LPA SYSTEM</div><div>{1}</div></div>";
        // 领导提醒
        const string email_tmpl_2 = @"<div style='font-family:Arial;font-size:10pt'><div>Dear {0},</div><div><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div><br></div><div>Your subordinate {2} has an LPA audit plan NOT FINISHED in LPA system which deadline is {3} , please push him(her) finish it as soon as possible.</div><br></blockquote></div><div><br></div><div>Thank you!<br></div><div><br></div><div>尊敬的{0}，</div><br><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div>你的下属{2}在LPA系统里你有1个LPA审核计划尚未完成，将于{3}截止，请督促他(她)尽快完成该计划。</div><br></blockquote><br><div>谢谢！</div><div><br></div><div>LPA SYSTEM</div><div>{1}</div></div>";

        // 问题未处理、本人提醒
        const string email_tmpl_3 = @"<div style='font-family:Arial;font-size:10pt'><div>Dear {0},</div><div><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div><br></div><div>There is/are {2} pcs problems <b>NOT FINISHED</b> in LPA system which deadline is <b>{3}</b> ,&nbsp;please check and close it/them as soon as possible.</div><br/><div>If you have any doubt about the problems, please contact the finder(s) or review it/them during the weekly LPA meeting.</div></blockquote></div><div><br></div><div>Thank you!<br/></div><div><br></div><div>尊敬的{0}，</div><br/><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div>LPA系统里你有{2}个问题点尚<b>未完成</b>，将于<b>{3}</b>截止，请尽快完成并关闭该问题。</div><br/><div>若对该问题有任何疑问，请直接联系问题发现人或LPA周会讨论.<br/></div></blockquote><br><div>谢谢！</div><div><br></div><div>LPA SYSTEM</div><div>{1}</div></div>";
        // 问题未处理、领导提醒
        const string email_tmpl_4_1 = @"<div>Your subordinate {0} has {1} pcs problems NOT FINISHED in LPA system, please push him(her) finish it/them as soon as possible.</div>";
        const string email_tmpl_4_2 = @"<div>你的下属{0}在LPA系统里你有{1}个问题点尚未完成，请督促他(她)尽快完成该问题。</div>";
        const string email_tmpl_4_3 = @"<div style='font-family:Arial;font-size:10pt'><div>Dear {0},</div><div><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div><br></div>{2}<br></blockquote></div><div><br></div><div>Thank you!<br></div><div><br></div><div>尊敬的{0}，</div><br><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'>{3}<br></blockquote><br><div>谢谢！</div><div><br></div><div>LPA SYSTEM</div><div>{1}</div></div>";

        //提醒新增的不符合项的负责人
        const string email_tmpl_5 =@"<div style='font-family:Arial;font-size:10pt'><div>Dear {0},</div><div><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div><br></div><div>You have {2} audit that does not qualify in LPA system.</div><br/></blockquote></div><div><br></div><div>Thank you!<br/></div><div><br></div><div>尊敬的{0}，</div><br/><blockquote style='margin: 0 0 0 40px; border: none; padding: 0px;'><div>LPA 系统里你有{2}项审核不符合项，请登录到您的LPA在线系统中查看该不符合项。</div><br/><div><a href='{3}' target='_blank' title='登录后台'>{3}</a><br/></div></blockquote><br><div>谢谢！</div><div><br></div><div>LPA SYSTEM</div><div>{1}</div></div>";

    }

}
