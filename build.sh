clear


rm --force --recursive IotDevices

ls -1 -lah

sudo git clone https://github.com/trmcnealy/IotDevices.git

sudo chmod 777 --recursive IotDevices

# dotnet build RaspberryPiDevices/RaspberryPiDevices.csproj --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false --output ./trmcnealy/Tests/bin/
#dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --no-restore --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false


#cd ~/Tests/IotDevices/RaspberryPiDevices.Tests/bin/Debug/net8.0/linux-arm64/
cd ~/Tests/IotDevices/

#dotnet restore

#dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --no-restore --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false --use-current-runtime false
dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --configuration Debug --framework net8.0 --arch arm64 --os linux --self-contained false

dotnet run --no-restore --project RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --property:Configuration=Debug

#dotnet run RaspberryPiDevices.Tests.dll

cd ~/Tests/IotDevices/
