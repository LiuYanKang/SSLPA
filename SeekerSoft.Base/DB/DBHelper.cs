using SeekerSoft.Core.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SeekerSoft.Base.DB
{
    public static class DBHelper
    {
        /// <summary>
        /// 创建一个新的数据库上下文
        /// </summary>
        /// <returns></returns>
        public static BaseEntities NewDB()
        {
            return new BaseEntities();
        }
    }
}
