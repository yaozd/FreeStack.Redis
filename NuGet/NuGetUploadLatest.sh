#!/bin/bash
ver=$(git describe --abbrev=0)
rm -rf packages
mkdir -p packages
mono ../src/.nuget/NuGet.exe pack ServiceStack.Redis/servicestack.redis.nuspec -Version $ver -symbols -OutputDirectory packages -BasePath ../
mono ../src/.nuget/NuGet.exe push "packages/FreeStack.Redis.$ver.nupkg"
mono ../src/.nuget/NuGet.exe push "packages/FreeStack.Redis.$ver.symbols.nupkg"
