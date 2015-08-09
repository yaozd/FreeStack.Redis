// Copyright (c) Service Stack LLC. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt


using NUnit.Framework;
using ServiceStack.Text;

namespace ServiceStack.Redis.Tests
{
    [TestFixture]
    public class FreeLicenseUsageTests : LicenseUsageTests
    {
        [SetUp]
        public void SetUp()
        {
            LicenseUtils.RemoveLicense();
            JsConfig.Reset();
        }

        [Test]
        public void Allows_access_of_20_types()
        {
            using (var client = new RedisClient(TestConfig.SingleHost))
            {
                Access20Types();
                Access20Types();

                Assert.Pass();
            }
        }

        [Test]
        public void Allows_access_of_21_types()
        {
            using (var client = new RedisClient(TestConfig.SingleHost))
            {
                Access20Types();
                Access20Types();

                client.As<T21>();

                Assert.Pass();
            }
        }

        [Test]
        public void Allows_access_of_more_than_21_types()
        {
            using (var client = new RedisClient(TestConfig.SingleHost))
            {
                Access20Types();
                Access20Types();

                client.As<T21>();
                Assert.Pass();

                client.As<T22>();
                Assert.Pass();

                client.As<T23>();
                Assert.Pass();

                client.As<T24>();
                Assert.Pass();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(6000)]
        [TestCase(6001)]
        [TestCase(6100)]
        [TestCase(8000)]
        [TestCase(100000)]
        public void Allows_access_of_any_number_of_operations(int times)
        {
            using (var client = new RedisClient(TestConfig.SingleHost))
            {
                times.Times(() => client.Get("any key"));
                Assert.Pass();
            }
        }
    }

    class T01 { public int Id { get; set; } }
    class T02 { public int Id { get; set; } }
    class T03 { public int Id { get; set; } }
    class T04 { public int Id { get; set; } }
    class T05 { public int Id { get; set; } }
    class T06 { public int Id { get; set; } }
    class T07 { public int Id { get; set; } }
    class T08 { public int Id { get; set; } }
    class T09 { public int Id { get; set; } }
    class T10 { public int Id { get; set; } }
    class T11 { public int Id { get; set; } }
    class T12 { public int Id { get; set; } }
    class T13 { public int Id { get; set; } }
    class T14 { public int Id { get; set; } }
    class T15 { public int Id { get; set; } }
    class T16 { public int Id { get; set; } }
    class T17 { public int Id { get; set; } }
    class T18 { public int Id { get; set; } }
    class T19 { public int Id { get; set; } }
    class T20 { public int Id { get; set; } }
    class T21 { public int Id { get; set; } }
    class T22 { public int Id { get; set; } }
    class T23 { public int Id { get; set; } }
    class T24 { public int Id { get; set; } }

    public class LicenseUsageTests
    {
        protected void Access20Types()
        {
            using (var client = new RedisClient(TestConfig.SingleHost))
            {
                client.As<T01>();
                client.As<T02>();
                client.As<T03>();
                client.As<T04>();
                client.As<T05>();
                client.As<T06>();
                client.As<T07>();
                client.As<T08>();
                client.As<T09>();
                client.As<T10>();
                client.As<T11>();
                client.As<T12>();
                client.As<T13>();
                client.As<T14>();
                client.As<T15>();
                client.As<T16>();
                client.As<T17>();
                client.As<T18>();
                client.As<T19>();
                client.As<T20>();
            }
        }
    }
}