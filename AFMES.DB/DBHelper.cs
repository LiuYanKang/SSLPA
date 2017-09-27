using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSLPA.DB
{
    public class DBHelper
    {
        /// <summary>
        /// 创建一个新的数据库上下文
        /// </summary>
        /// <returns></returns>
        public static AFMESEntities NewDB()
        {
            return new AFMESEntities();
        }
    }
}
