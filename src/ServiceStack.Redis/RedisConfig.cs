﻿using System;
using System.Net.Security;
using System.Threading;

namespace ServiceStack.Redis
{
    public class RedisConfig
    {
        //redis-server defaults:
        public const long DefaultDb = 0;
        public const int DefaultPort = 6379;
        public const int DefaultPortSsl = 6380;
        public const int DefaultPortSentinel = 26379;
        public const string DefaultHost = "localhost";

        public static Func<RedisEndpoint, RedisClient> ClientFactory = c =>
        {
            Interlocked.Increment(ref RedisState.TotalClientsCreated);
            return new RedisClient(c);
        };

        public static int DefaultConnectTimeout = 0;
        public static int DefaultSendTimeout = -1;
        public static int DefaultReceiveTimeout = -1;
        public static int DefaultRetryTimeout = 3 * 1000;
        public static int DefaultIdleTimeOutSecs = 240;
        public static int BackOffMultiplier = 10;

        public static int BufferLength = 1450;

        public static int BufferPoolMaxSize = 500000;

        public static bool VerifyMasterConnections = true;

        public static int HostLookupTimeoutMs = 200;

        //Skip ServerVersion Checks by specifying Min Version number, e.g: 2.8.12 => 2812, 2.9.1 => 2910
        public static int? AssumeServerVersion;

        public static TimeSpan DeactivatedClientsExpiry = TimeSpan.FromMinutes(1);

        public static bool DisableVerboseLogging = false;

        //Example at: http://msdn.microsoft.com/en-us/library/office/dd633677(v=exchg.80).aspx 
        public static LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }
        public static RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }

        /// <summary>
        /// Resets Redis Config and Redis Stats back to default values
        /// </summary>
        public static void Reset()
        {
            RedisStats.Reset();

            DefaultConnectTimeout = 0;
            DefaultSendTimeout = -1;
            DefaultReceiveTimeout = -1;
            DefaultRetryTimeout = 3 * 1000;
            DefaultIdleTimeOutSecs = 240;
            BackOffMultiplier = 10;
            BufferLength = 1450;
            BufferPoolMaxSize = 500000;
            VerifyMasterConnections = true;
            HostLookupTimeoutMs = 200;
            AssumeServerVersion = null;
            DeactivatedClientsExpiry = TimeSpan.FromMinutes(1);
            DisableVerboseLogging = false;
            CertificateSelectionCallback = null;
            CertificateValidationCallback = null;
        }
    }
}