using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_WebApp.CustomSessions
{
    public static class CustomSessionExtension
    {
        public static void SetSessionData<T>(this ISession session, string sessionKey, T sessionValue)
        {
            session.SetString(sessionKey, JsonConvert.SerializeObject(sessionValue));
        }

        public static T GetSessionData<T>(this ISession session, string sessionKey)
        {
            var data = session.GetString(sessionKey);
            if (data == null)
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
        }
    }
}
