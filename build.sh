# dotnet build RaspberryPiDevices/RaspberryPiDevices.csproj --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false --output ./trmcnealy/Tests/bin/
#dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --no-restore --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false

#cd ~/Tests/IotDevices/RaspberryPiDevices.Tests/bin/Debug/net8.0/linux-arm64/
cd ~/Tests/IotDevices/

dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --no-restore --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false --use-current-runtime false

dotnet run --no-restore --project /RaspberryPiDevices.TestsRaspberryPiDevices.Tests.csproj --property:Configuration=Debug --property:Framework=net8.0 -property:Runtime=linux-arm64

#dotnet run RaspberryPiDevices.Tests.dll

cd ~/Tests/IotDevices/
