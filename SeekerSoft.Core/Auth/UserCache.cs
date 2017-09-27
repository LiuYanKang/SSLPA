using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekerSoft.Core.Auth
{
    /// <summary>
    /// 用户信息缓存
    /// </summary>
    public class UserCache
    {
        private static readonly ConcurrentDictionary<Guid, UserInfo> UserInfoBuff = new ConcurrentDictionary<Guid, UserInfo>();


        public static UserInfo GetByTicket(Guid ticket)
        {
            UserInfo userinfo;
            UserInfoBuff.TryGetValue(ticket, out userinfo);
            return userinfo;
        }

        public static UserInfo GetByID(int id, string terminal, UserType userType = UserType.Staff)
        {
            return UserInfoBuff.Where(k => k.Value.UserID == id && k.Value.UserType == userType && k.Value.Terminal == terminal).Select(k => k.Value).FirstOrDefault();
        }

        /// <summary>
        /// 根据凭据移除用户信息
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static bool RemoveByTicket(Guid ticket)
        {
            UserInfo userinfo;
            return UserInfoBuff.TryRemove(ticket, out userinfo);
        }

        public static bool Add(UserInfo userinfo)
        {
            return UserInfoBuff.TryAdd(userinfo.Ticket, userinfo);
        }


        public static void ClearCache()
        {
            UserInfoBuff.Clear();
        }
    }
}
