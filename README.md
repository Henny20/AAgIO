# AAgIO

AAgOpenGPS is a direct port of Brian Tischler's [AgOpenGPS] (https://github.com/farmerbriantee/AgOpenGPS) using the multiplatform Avalonia framework (https://avaloniaui.net)
Tested on Fedora37.

Much of the code is 
Copyright Brian Tischler.



<h3>How to build on Fedora(Debug)</h3>

```
sudo dnf install dotnet-sdk-6.0

cd AAgIO

dotnet build (or dotnet run for the impatient)

./bin/Debug/net6.0/AAgIO
```

Building on dotnet8: Change AAgIO.csproj
<TargetFramework>net8.0</TargetFramework>

