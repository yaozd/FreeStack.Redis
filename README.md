## Fork of the ServiceStack C#/.NET Client for Redis

This is a fork of the official C# ServiceStack client for Redis. This fork removes

* the limitation of 6000 commands per hour to the Redis Client
* the limitation of 20 different types in Redis Client Typed APIs 

both imposed to the free version of the official library.

You can install this fork <a href="https://www.nuget.org/packages/FreeStack.Redis/" target="_blank">from NuGet.org</a>. You can use the NuGet Ui in VS or Xamarin or you can run the following command in the nuget package manager console

<<<<<<< HEAD
=======
Some examples of supported formats:

    localhost
    127.0.0.1:6379
    redis://localhost:6379
    password@localhost:6379
    clientid:password@localhost:6379
    redis://clientid:password@localhost:6380?ssl=true&db=1

> More examples can be seen in 
[ConfigTests.cs](https://github.com/ServiceStack/ServiceStack.Redis/blob/master/tests/ServiceStack.Redis.Tests/ConfigTests.cs)

Any additional configuration can be specified as QueryString parameters. The full list of options that can be specified include:

<table>
    <tr>
        <td><b>Ssl</b></td>
        <td>bool</td>
        <td>If this is an SSL connection</td>
    </tr>
    <tr>
        <td><b>Db</b></td>
        <td>int</td>
        <td>The Redis DB this connection should be set to</td>
    </tr>
    <tr>
        <td><b>Client</b></td>
        <td>string</td>
        <td>A text alias to specify for this connection for analytic purposes</td>
    </tr>
    <tr>
        <td><b>Password</b></td>
        <td>string</td>
        <td>UrlEncoded version of the Password for this connection</td>
    </tr>
    <tr>
        <td><b>ConnectTimeout</b></td>
        <td>int</td>
        <td>Timeout in ms for making a TCP Socket connection</td>
    </tr>
    <tr>
        <td><b>SendTimeout</b></td>
        <td>int</td>
        <td>Timeout in ms for making a synchronous TCP Socket Send</td>
    </tr>
    <tr>
        <td><b>ReceiveTimeout</b></td>
        <td>int</td>
        <td>Timeout in ms for waiting for a synchronous TCP Socket Receive</td>
    </tr>
    <tr>
        <td><b>IdleTimeOutSecs</b></td>
        <td>int</td>
        <td>Timeout in Seconds for an Idle connection to be considered active</td>
    </tr>
    <tr>
        <td><b>NamespacePrefix</b></td>
        <td>string</td>
        <td>Use a custom prefix for ServiceStack.Redis internal index colletions</td>
    </tr>
</table>

## Redis Client Managers

The recommended way to access `RedisClient` instances is to use one of the available Thread-Safe Client Managers below. Client Managers are connection factories which is ideally registered as a Singleton either in your IOC or static classes. 

### RedisManagerPool

With the enhanced Redis URI Connection Strings we've been able to simplify and streamline the existing `PooledRedisClientManager` implementation and have extracted it out into a new clients manager called `RedisManagerPool`. 

In addition to removing all above options on the Client Manager itself, readonly connection strings have also been removed so the configuration ends up much simpler and more aligned with the common use-case:

```csharp
container.Register<IRedisClientsManager>(c => 
    new RedisManagerPool(redisConnectionString));
```

**Pooling Behavior**

Any connections required after the maximum Pool size has been reached will be created and disposed outside of the Pool. By not being restricted to a maximum pool size, the pooling behavior in `RedisManagerPool` can maintain a smaller connection pool size at the cost of potentially having a higher opened/closed connection count.

### PooledRedisClientManager

If you prefer to define options on the Client Manager itself or you want to provide separate Read/Write and ReadOnly 
(i.e. Master and Slave) redis-servers, use the `PooledRedisClientManager` instead:

```csharp
container.Register<IRedisClientsManager>(c => 
    new PooledRedisClientManager(redisReadWriteHosts, redisReadOnlyHosts) { 
        ConnectTimeout = 100,
        //...
    });
```

**Pooling Behavior**

The `PooledRedisClientManager` imposes a maximum connection limit and when its maximum pool size has been reached will instead block on any new connection requests until the next `RedisClient` is released back into the pool. If no client became available within `PoolTimeout`, a Pool `TimeoutException` will be thrown.

### BasicRedisClientManager

If don't want to use connection pooling (i.e. your accessing a local redis-server instance) you can use a basic (non-pooled) Clients Manager which creates a new `RedisClient` instance each time:

```csharp
container.Register<IRedisClientsManager>(c => 
    new BasicRedisClientManager(redisConnectionString));
```

### Accessing the Redis Client

Once registered, accessing the RedisClient is the same in all Client Managers, e.g:

```csharp
var clientsManager = container.Resolve<IRedisClientsManager>();
using (IRedisClient redis = clientsManager.GetClient())
{
    redis.IncrementValue("counter");
    List<string> days = redis.GetAllItemsFromList("days");

    //Access Typed API
    var redisTodos = redis.As<Todo>();

    redisTodos.Store(new Todo {
        Id = redisTodos.GetNextSequence(),
        Content = "Learn Redis",
    });

    var todo = redisTodos.GetById(1);

    //Access Native Client
    var redisNative = (IRedisNativeClient)redis;

    redisNative.Incr("counter");
    List<string> days = redisNative.LRange("days", 0, -1);
}
```

A more detailed list of the available RedisClient API's used in the example can be seen in the C# interfaces below:

 - [IRedisClient](https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.Interfaces/Redis/IRedisClient.cs)
 - [IRedisTypedClient<T>](https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.Interfaces/Redis/Generic/IRedisTypedClient.cs)
 - [IRedisNativeClient](https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.Interfaces/Redis/IRedisNativeClient.cs)

## [Redis React Browser](https://servicestack.net/redis-react)

Redis React is a simple user-friendly UI for browsing data in Redis servers which takes advantages of the complex
type conventions built in the ServiceStack.Redis Client to provide a rich, human-friendly UI for navigating related datasets, enabling a fast and fluid browsing experience for your Redis servers.

#### [Live Demo](http://redisreact.servicestack.net/#/)

[![](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/livedemos/redis-react/home.png)](http://redisreact.servicestack.net/#/)

#### Downloads available from [Redis React Home Page](https://servicestack.net/redis-react)

## [Redis Sentinel](https://github.com/ServiceStack/ServiceStack.Redis/wiki/Redis-Sentinel)

To use the new Sentinel support, instead of populating the Redis Client Managers with the 
connection string of the master and slave instances you would create a single RedisSentinel 
instance configured with the connection string of the running Redis Sentinels:

```csharp
var sentinelHosts = new[]{ "sentinel1", "sentinel2:6390", "sentinel3" };
var sentinel = new RedisSentinel(sentinelHosts, masterName: "mymaster");
```

This configues a `RedisSentinel` with 3 sentinel hosts looking at **mymaster** group. 
As the default port for sentinels when unspecified is **26379** and how RedisSentinel is able to 
auto-discover other sentinels, the minimum configuration required is with a single Sentinel host:

```csharp
var sentinel = new RedisSentinel("sentinel1");
```

### Custom Redis Connection String

The host the RedisSentinel is configured with only applies to that Sentinel Host, to use the 
flexibility of [Redis Connection Strings](#redis-connection-strings) to apply configuration on
individual Redis Clients you need to register a custom `HostFilter`:

```csharp
sentinel.HostFilter = host => "{0}?db=1&RetryTimeout=5000".Fmt(host);
```

An alternative to using connection strings for configuring clients is to modify 
[default configuration on RedisConfig](https://github.com/ServiceStack/ServiceStack.Redis/wiki/Redis-Config).

### Change to use RedisManagerPool 

By default RedisSentinel uses a `PooledRedisClientManager`, this can be changed to use the 
newer `RedisManagerPool` with:

```csharp
sentinel.RedisManagerFactory = (master,slaves) => new RedisManagerPool(master);
```

### Start monitoring Sentinels

Once configured, you can start monitoring the Redis Sentinel servers and access the pre-configured 
client manager with:

```csharp
IRedisClientsManager redisManager = sentinel.Start();
```

Which as before, can be registered in your preferred IOC as a **singleton** instance:

```csharp
container.Register<IRedisClientsManager>(c => sentinel.Start());
```

## [Configure Redis Sentinel Servers](https://github.com/ServiceStack/redis-config)

[![Instant Redis Setup](https://raw.githubusercontent.com/ServiceStack/Assets/master/img/redis/instant-sentinel-setup.png)](https://github.com/ServiceStack/redis-config)

See the
[redis config project](https://github.com/ServiceStack/redis-config) for a quick way to setup up 
the minimal 
[highly available Redis Sentinel configuration](https://github.com/ServiceStack/redis-config/blob/master/README.md#3x-sentinels-monitoring-1x-master-and-2x-slaves)
including start/stop scripts for instantly running multiple redis instances on a single (or multiple) 
Windows, OSX or Linux servers. 

### [Redis Stats](https://github.com/ServiceStack/ServiceStack.Redis/wiki/Redis-Stats)

You can use the `RedisStats` class for visibility and introspection into your running instances.
The [Redis Stats wiki](https://github.com/ServiceStack/ServiceStack.Redis/wiki/Redis-Stats) lists the stats available.

## [Automatic Retries](https://github.com/ServiceStack/ServiceStack.Redis/wiki/Automatic-Retries)

To improve the resilience of client connections, `RedisClient` will transparently retry failed 
Redis operations due to Socket and I/O Exceptions in an exponential backoff starting from 
**10ms** up until the `RetryTimeout` of **3000ms**. These defaults can be tweaked with:

```csharp
RedisConfig.DefaultRetryTimeout = 3000;
RedisConfig.BackOffMultiplier = 10;
```

## [ServiceStack.Redis SSL Support](https://github.com/ServiceStack/ServiceStack/wiki/Secure-SSL-Redis-connections-to-Azure-Redis)

ServiceStack.Redis now supporting **SSL connections** making it suitable for accessing remote Redis server instances over a 
**secure SSL connection**.

![Azure Redis Cache](https://github.com/ServiceStack/Assets/raw/master/img/wikis/redis/azure-redis-instance.png)

### [Connecting to Azure Redis](https://github.com/ServiceStack/ServiceStack/wiki/Secure-SSL-Redis-connections-to-Azure-Redis)

As connecting to [Azure Redis Cache](http://azure.microsoft.com/en-us/services/cache/) via SSL was the primary use-case for this feature, 
we've added a new 
[Getting connected to Azure Redis via SSL](https://github.com/ServiceStack/ServiceStack/wiki/Secure-SSL-Redis-connections-to-Azure-Redis) 
to help you get started.

## New Generic API's for calling Custom Redis commands

Most of the time when waiting to use a new [Redis Command](http://redis.io/commands) you'll need to wait for an updated version of 
**ServiceStack.Redis** to add support for the new commands likewise there are times when the Redis Client doesn't offer every permutation 
that redis-server supports. 

With the new `Custom` and `RawCommand` API's on `IRedisClient` and `IRedisNativeClient` you can now use the RedisClient to send your own 
custom commands that can call adhoc Redis commands:

```csharp
public interface IRedisClient
{
    ...
    RedisText Custom(params object[] cmdWithArgs);
}

public interface IRedisNativeClient
{
    ...
    RedisData RawCommand(params object[] cmdWithArgs);
    RedisData RawCommand(params byte[][] cmdWithBinaryArgs);
}
```

These Custom API's take a flexible `object[]` arguments which accepts any serializable value e.g. 
`byte[]`, `string`, `int` as well as any user-defined Complex Types which are transparently serialized 
as JSON and send across the wire as UTF-8 bytes. 

```csharp
var ret = Redis.Custom("SET", "foo", 1);          // ret.Text = "OK"

byte[] cmdSet = Commands.Set;
ret = Redis.Custom(cmdSet, "bar", "b");           // ret.Text = "OK"

ret = Redis.Custom("GET", "foo");                 // ret.Text = "1"
```

There are also 
[convenient extension methods](https://github.com/ServiceStack/ServiceStack.Redis/blob/master/src/ServiceStack.Redis/RedisDataExtensions.cs) 
on `RedisData` and `RedisText` that make it easy to access structured data, e.g:

```csharp
var ret = Redis.Custom(Commands.Keys, "*");
var keys = ret.GetResults();                      // keys = ["foo", "bar"]

ret = Redis.Custom(Commands.MGet, "foo", "bar");
var values = ret.GetResults();                    // values = ["1", "b"]

Enum.GetNames(typeof(DayOfWeek)).ToList()
    .ForEach(x => Redis.Custom(Commands.RPush, "DaysOfWeek", x));
ret = Redis.Custom(Commands.LRange, "DaysOfWeek", 1, -2);
var weekDays = ret.GetResults();      

weekDays.PrintDump(); // ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
```

and some more examples using Complex Types with the Custom API's:

```csharp
var ret = Redis.Custom(Commands.Set, "foo", new Poco { Name = "Bar" }); // ret.Text = "OK"

ret = Redis.Custom(Commands.Get, "foo");          // ret.Text =  {"Name":"Bar"}
Poco dto = ret.GetResult<Poco>();

dto.Name.Print(); // Bar
```

## New Managed Pub/Sub Server 

The Pub/Sub engine powering 
[Redis ServerEvents](https://github.com/ServiceStack/ServiceStack/wiki/Redis-Server-Events) and 
[Redis MQ](https://github.com/ServiceStack/ServiceStack/wiki/Messaging-and-Redis) has been extracted 
and encapsulated it into a re-usable class that can be used independently for handling messages 
published to specific [Redis Pub/Sub](http://redis.io/commands#pubsub) channels. 

`RedisPubSubServer` processes messages in a managed background thread that **automatically reconnects** 
when the redis-server connection fails and works like an independent background Service that can be 
stopped and started on command. 

The public API is captured in the 
[IRedisPubSubServer](https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.Interfaces/Redis/IRedisPubSubServer.cs) interface:

```csharp
public interface IRedisPubSubServer : IDisposable
{
    IRedisClientsManager ClientsManager { get; }
    // What Channels it's subscribed to
    string[] Channels { get; }

    // Run once on initial StartUp
    Action OnInit { get; set; }
    // Called each time a new Connection is Started
    Action OnStart { get; set; }
    // Invoked when Connection is broken or Stopped
    Action OnStop { get; set; }
    // Invoked after Dispose()
    Action OnDispose { get; set; }

    // Fired when each message is received
    Action<string, string> OnMessage { get; set; }
    // Fired after successfully subscribing to the specified channels
    Action<string> OnUnSubscribe { get; set; }
    // Called when an exception occurs 
    Action<Exception> OnError { get; set; }
    // Called before attempting to Failover to a new redis master
    Action<IRedisPubSubServer> OnFailover { get; set; }

    int? KeepAliveRetryAfterMs { get; set; }
    // The Current Time for RedisServer
    DateTime CurrentServerTime { get; }

    // Current Status: Starting, Started, Stopping, Stopped, Disposed
    string GetStatus();
    // Different life-cycle stats
    string GetStatsDescription();
    
    // Subscribe to specified Channels and listening for new messages
    IRedisPubSubServer Start();
    // Close active Connection and stop running background thread
    void Stop();
    // Stop than Start
    void Restart();
}
```
### Usage 

To use `RedisPubSubServer`, initialize it with the channels you want to subscribe to and assign handlers 
for each of the events you want to handle. At a minimum you'll want to handle `OnMessage`:

```csharp
var clientsManager = new PooledRedisClientManager();
var redisPubSub = new RedisPubSubServer(clientsManager, "channel-1", "channel-2") {
        OnMessage = (channel, msg) => "Received '{0}' from '{1}'".Print(msg, channel)
    }.Start();
```

Calling `Start()` after it's initialized will get it to start listening and processing any messages 
published to the subscribed channels.

### New Lex Operations

The new [ZRANGEBYLEX](http://redis.io/commands/zrangebylex) sorted set operations allowing you to query a sorted set lexically have been added. 
A good showcase for this is available on [autocomplete.redis.io](http://autocomplete.redis.io/).

These new operations are available as a 1:1 mapping with redis-server on `IRedisNativeClient`:

```csharp
public interface IRedisNativeClient
{
    ...
    byte[][] ZRangeByLex(string setId, string min, string max, int? skip=null, int? take=null);
    long ZLexCount(string setId, string min, string max);
    long ZRemRangeByLex(string setId, string min, string max);
}
```

And the more user-friendly APIs under `IRedisClient`:

```csharp
public interface IRedisClient
{
    ...
    List<string> SearchSortedSet(string setId, string start=null, string end=null);
    long SearchSortedSetCount(string setId, string start=null, string end=null);
    long RemoveRangeFromSortedSetBySearch(string setId, string start=null, string end=null);
}
```

Just like NuGet version matchers, Redis uses `[` char to express inclusiveness and `(` char for exclusiveness.
Since the `IRedisClient` APIs defaults to inclusive searches, these two APIs are the same:

```csharp
Redis.SearchSortedSetCount("zset", "a", "c")
Redis.SearchSortedSetCount("zset", "[a", "[c")
```

Alternatively you can specify one or both bounds to be exclusive by using the `(` prefix, e.g:

```csharp
Redis.SearchSortedSetCount("zset", "a", "(c")
Redis.SearchSortedSetCount("zset", "(a", "(c")
```

More API examples are available in [LexTests.cs](https://github.com/ServiceStack/ServiceStack.Redis/blob/master/tests/ServiceStack.Redis.Tests/LexTests.cs).

### New HyperLog API

The development branch of Redis server (available when v3.0 is released) includes an ingenious algorithm to approximate the unique elements in a set with maximum space and time efficiency. For details about how it works see Redis's creator Salvatore's blog who [explains it in great detail](http://antirez.com/news/75). Essentially it lets you maintain an efficient way to count and merge unique elements in a set without having to store its elements. 
A Simple example of it in action:

```csharp
redis.AddToHyperLog("set1", "a", "b", "c");
redis.AddToHyperLog("set1", "c", "d");
var count = redis.CountHyperLog("set1"); //4

redis.AddToHyperLog("set2", "c", "d", "e", "f");

redis.MergeHyperLogs("mergedset", "set1", "set2");

var mergeCount = redis.CountHyperLog("mergedset"); //6
```

### New Scan APIs Added

Redis v2.8 introduced a beautiful new [SCAN](http://redis.io/commands/scan) operation that provides an optimal strategy for traversing a redis instance entire keyset in managable-size chunks utilizing only a client-side cursor and without introducing any server state. It's a higher performance alternative and should be used instead of [KEYS](http://redis.io/commands/keys) in application code. SCAN and its related operations for traversing members of Sets, Sorted Sets and Hashes are now available in the Redis Client in the following API's:

```csharp
public interface IRedisClient
{
    ...
    IEnumerable<string> ScanAllKeys(string pattern = null, int pageSize = 1000);
    IEnumerable<string> ScanAllSetItems(string setId, string pattern = null, int pageSize = 1000);
    IEnumerable<KeyValuePair<string, double>> ScanAllSortedSetItems(string setId, string pattern = null, int pageSize = 1000);
    IEnumerable<KeyValuePair<string, string>> ScanAllHashEntries(string hashId, string pattern = null, int pageSize = 1000);    
}

//Low-level API
public interface IRedisNativeClient
{
    ...
    ScanResult Scan(ulong cursor, int count = 10, string match = null);
    ScanResult SScan(string setId, ulong cursor, int count = 10, string match = null);
    ScanResult ZScan(string setId, ulong cursor, int count = 10, string match = null);
    ScanResult HScan(string hashId, ulong cursor, int count = 10, string match = null);
}
>>>>>>> upstream/master
```
PM> Install-Package FreeStack.Redis
```

For documentation and commercial support please visit the <a href="https://github.com/ServiceStack/ServiceStack.Redis" target="_blank">ServiceStack.Redis repo</a>.
