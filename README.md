## Fork of the ServiceStack C#/.NET Client for Redis

This is a fork of the official C# ServiceStack client for Redis. This fork removes

* the limitation of 6000 commands per hour to the Redis Client
* the limitation of 20 different types in Redis Client Typed APIs 

both imposed to the free version of the official library.

You can install this fork <a href="https://www.nuget.org/packages/FreeStack.Redis/" target="_blank">from NuGet.org</a>. You can use the NuGet Ui in VS or Xamarin or you can run the following command in the nuget package manager console

```
PM> Install-Package FreeStack.Redis
```

For documentation and commercial support please visit the <a href="https://github.com/ServiceStack/ServiceStack.Redis" target="_blank">ServiceStack.Redis repo</a>.
