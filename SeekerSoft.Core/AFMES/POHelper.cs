using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.AFMES
{
    public class POHelper
    {
        /// <summary>
        /// 截取XP系统打印的订单号
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        public static string GetNoFromXP(string po)
        {
            if (string.IsNullOrEmpty(po)) return po;
            po = po.Trim();
            if (!po.StartsWith("F0") || !po.EndsWith("0000")) return po;

            po = po.Substring(2, 6);

            return po;
        }
    }
}
