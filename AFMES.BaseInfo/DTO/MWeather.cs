using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    public class MWeather
    {
        public System.DateTime WeatherTime { get; set; }
        public string City { get; set; }
        public string Temp { get; set; }
        public string TodayTemp { get; set; }
        public string WeatherInfo { get; set; }
        public string Icon { get; set; }
    }
}
