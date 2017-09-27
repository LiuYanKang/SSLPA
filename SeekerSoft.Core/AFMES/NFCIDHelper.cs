using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.AFMES
{
    public class NFCIDHelper
    {
        /// <summary>
        /// 转换NFCID为标准数字
        /// </summary>
        /// <param name="nfcid"></param>
        /// <returns></returns>
        public static string ToStand(string nfcid)
        {
            // 56 5D D2 4D
            // 4D D2 5D 56
            // 1305632086
            char[] cArr = new char[nfcid.Length];
            cArr[0] = nfcid[6];
            cArr[1] = nfcid[7];
            cArr[2] = nfcid[4];
            cArr[3] = nfcid[5];
            cArr[4] = nfcid[2];
            cArr[5] = nfcid[3];
            cArr[6] = nfcid[0];
            cArr[7] = nfcid[1];
            string revArr = new string(cArr);
            UInt32 dex = Convert.ToUInt32(revArr, 16);
            return dex.ToString("D10");
        }
    }
}
