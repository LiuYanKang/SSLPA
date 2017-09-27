using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.Json
{
    public class JsonHelper
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject(json, typeof(T)) as T;
        }

    }
}
