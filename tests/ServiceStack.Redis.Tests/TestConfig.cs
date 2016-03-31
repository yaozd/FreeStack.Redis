using System;
using ServiceStack.Logging;
using ServiceStack.Support;

namespace ServiceStack.Redis.Tests
{
    public static class TestConfig
    {
        static TestConfig()
        {
            LogManager.LogFactory = new InMemoryLogFactory();
        }

        public const bool IgnoreLongTests = true;

        public static string SingleHost
        {
            get { return Environment.GetEnvironmentVariable("CI_REDIS") ?? "localhost"; }
        }
        public static readonly string[] MasterHosts = new[] { "localhost" };
        public static readonly string[] SlaveHosts = new[] { "localhost" };

        public const int RedisPort = 6379;

        public static string SingleHostConnectionString
        {
            get
            {
                return SingleHost + ":" + RedisPort;
            }
        }

        public static BasicRedisClientManager BasicClientManger
        {
            get
            {
                return new BasicRedisClientManager(new[] {
                    SingleHostConnectionString
                });
            }
        }
    }
}