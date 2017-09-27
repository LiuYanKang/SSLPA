using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.Config
{
    public class ConfigHelper
    {
        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            int result = 0;
            if (int.TryParse(Get(key), out result))
                return result;
            else
                return defaultValue;
        }


        public static void Set(string key, string value)
        {
            // 配置文件保存
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
            config.Save();
        }
    }
}
