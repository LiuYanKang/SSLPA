using SSLPA.BaseInfo.DTO;
using SSLPA.DB;
using SeekerSoft.Core.DB;
using SeekerSoft.Core.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.BL
{
    /// <summary>
    /// 天气数据管理
    /// </summary>
    public class WeatherBL
    {
        static string appkey = "b91f8b1497faf08f5288f70efbb6e3ef"; //配置您申请的appkey
        static string cityCode = "1363";// 太仓

        static string wsUrl = "http://v.juhe.cn/weather/index?cityname={1}&dtype=&format=&key={0}";

        /// <summary>
        /// 更新天气数据并存入数据库
        /// </summary>
        public static void Refresh()
        {
            try
            {
                string url = string.Format(wsUrl, appkey, cityCode);
                string resultJson = SeekerSoft.Core.Web.RequestUtility.HttpGet(url);

                var data = JsonHelper.Deserialize<WSResult>(resultJson);

                if (data.error_code == "0")
                {
                    SeekerSoft.Core.Log.Info(typeof(WeatherBL).ToString(), "天气获取成功。" + data.result.today.ToString());
                }
                else
                {
                    SeekerSoft.Core.Log.Error(typeof(WeatherBL).ToString(), "天气获取失败。" + data.error_code + ":" + data.reason);
                }

                using (var db = DBHelper.NewDB())
                {
                    Weather entity = new Weather()
                    {
                        City = data.result.today.city,
                        CreateTime = DateTime.Now,
                        WeatherTime = DateTime.Parse(data.result.today.date_y.Replace("年", "-").Replace("月", "-").Replace("日", " ") + data.result.sk.time),
                        Temp = data.result.sk.temp,
                        TodayTemp = data.result.today.temperature,
                        WeatherInfo = data.result.today.weather,
                        Icon1 = data.result.today.weather_id.fa,
                        Icon2 = data.result.today.weather_id.fb
                    };
                    db.Weather.Add(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                SeekerSoft.Core.Log.Error(typeof(WeatherBL).ToString(), ex.ToString());
            }
        }

        public static MWeather Query()
        {
            string sql = @"
SELECT  WeatherTime ,
        City ,
        Temp ,
        TodayTemp ,
        WeatherInfo ,
        Icon = Icon1
FROM    dic.Weather
WHERE CreateTime > DATEADD(DAY,-1,GETDATE())
ORDER BY CreateTime DESC ";
            // 一天以内更新过的数据有效

            using (var db = DBHelper.NewDB())
            {
                var data = DBHelper<MWeather>.ExecuteSqlToEntities(db, sql).FirstOrDefault();
                return data;
            }
        }
    }

    class WSResult
    {
        public string resultcode { get; set; }
        public string reason { get; set; }
        public Result result { get; set; }
        public string error_code { get; set; }

        public class Result
        {
            /// <summary>
            /// 当前实况天气
            /// </summary>
            public SK sk { get; set; }

            public Today today { get; set; }



            public class SK
            {
                /// <summary>
                /// 当前温度
                /// </summary>
                public string temp { get; set; }
                /// <summary>
                /// 当前湿度
                /// </summary>
                public string humidity { get; set; }
                /// <summary>
                /// 更新时间
                /// </summary>
                public string time { get; set; }
            }

            public class Today
            {
                /// <summary>
                /// 今日温度
                /// </summary>
                public string temperature { get; set; }
                /// <summary>
                /// 今日天气
                /// </summary>
                public string weather { get; set; }
                /// <summary>
                /// 日期
                /// </summary>
                public string date_y { get; set; }
                /// <summary>
                /// 城市
                /// </summary>
                public string city { get; set; }
                /// <summary>
                /// 天气唯一标识
                /// </summary>
                public WeatherId weather_id { get; set; }

                public class WeatherId
                {
                    public string fa { get; set; }
                    public string fb { get; set; }
                }

                public override string ToString()
                {
                    return "日期：" + date_y + ",城市：" + city + ",温度：" + temperature + ",天气：" + weather;
                }
            }
        }
    }
}
