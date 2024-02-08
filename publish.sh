
cd ~/Projects

rm -r *

dotnet publish [<PROJECT>|<SOLUTION>] --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false --output ./trmcnealy/Tests/publish/

dotnet publish -p:PublishAot=true -p:OptimizationPreference=Speed -p:StackTraceSupport=false -p:InvariantGlobalization=true -p:UseSystemResourceKeys=true







dotnet publish [<PROJECT>|<SOLUTION>] ^
--configuration Debug ^
--framework net8.0 ^
--runtime linux-arm64 ^
--self-contained false ^
--output E:/Github/trmcnealy/RPi/publish

dotnet publish E:/Github/trmcnealy/RPi/Rpi.Devices/Rpi.Devices.csproj ^
--configuration Debug ^
--framework net8.0 ^
--runtime linux-arm64 ^
--self-contained false ^
--output E:/Github/trmcnealy/RPi/publish ^
-p:UseSystemResourceKeys=true


dotnet publish E:/Github/trmcnealy/RPi/RPiUI/RPiUI.csproj ^
--configuration Debug ^
--framework net8.0 ^
--runtime linux-arm64 ^
--self-contained false ^
--output E:/Github/trmcnealy/RPi/publish

dotnet publish E:/Github/trmcnealy/RPi/RPiUI.Desktop/RPiUI.Desktop.csproj --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false --output //sshfs.r/trmcnealy@trmpi/home/trmcnealy/Workspace

-p:PublishAot=true ^
-p:OptimizationPreference=Speed ^
-p:StackTraceSupport=false ^
-p:InvariantGlobalization=true ^
-p:UseSystemResourceKeys=true


dotnet publish E:/Github/trmcnealy/RPi/RPiUI.Desktop/RPiUI.Desktop.csproj --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained true --output //sshfs.r/trmcnealy@trmpi/home/trmcnealy/Workspace