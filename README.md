# AAgIO

AAgOpenGPS is a direct port of Brian Tischler's [AgOpenGPS] (https://github.com/farmerbriantee/AgOpenGPS) using the multiplatform Avalonia framework (https://avaloniaui.net)
Tested on Fedora37.

Much of the code is 
Copyright Brian Tischler.



<h3>How to build on Fedora(Debug)</h3>

```bash
sudo dnf install dotnet-sdk-6.0

cd AAgIO

dotnet build

./bin/Debug/net6.0/AAgIO
```

Building on dotnet8: Change AAgIO.csproj
```xml
<TargetFramework>net8.0</TargetFramework>
```

<img src="https://github.com/Henny20/AAgIO/blob/main/Assets/Screenshot.png" width=50%  />

<h3>Android</h3>

<img src="https://github.com/Henny20/AAgIO/blob/main/Assets/Screenshot2.jpeg" width=50%  />

