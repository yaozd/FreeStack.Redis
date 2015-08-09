using ServiceStack.Configuration;

namespace ServiceStack.Redis.Tests.Utils
{
    public static class Configuration
    {
        public static string AppConfigValue(this string key)
        {
            return new AppSettings().GetString(key);
        }
    }
}

