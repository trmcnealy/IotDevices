clear

#curl -sSSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg


#puttygen.exe Generate private
#save 


ssh-keygen -t rsa
# -N "3698" -f ~/.ssh/authorized_keys
#ssh-copy-id -i ~/.ssh/id_rsa.pub
#ssh-copy-id -i ~/.ssh/id_rsa.pub -f trmcnealy@host

#ssh -i ~/.ssh/id_rsa.pub trmcnealy@trmpi

ssh -i %HOME%/.ssh/id_rsa.pub trmcnealy@trmpi

scp trmcnealy@trmpi:~/.ssh/id_rsa.pub %HOME%/.ssh/id_rsa.pub

scp -i D:/TFS_Sources/id_rsa.pub -r E:/Github/trmcnealy/IotDevices/RaspberryPiDevices.Tests/bin/publish/* trmcnealy@trmpi:~/Projects

scp -i ~/.ssh/id_rsa -r trmcnealy@TRMRIPPER:E:/Github/trmcnealy/IotDevices/RaspberryPiDevices.Tests/bin/publish/* ~/Projects

#scp -r D:/authorized_keys trmcnealy@trmpi:~/.ssh/
#scp -r D:/pi-sh-key.ppk trmcnealy@trmpi:~/.ssh/authorized_keys

#plink -i D:/pi-sh-key.ppk trmcnealy@trmpi -batch -T echo "hello world"
#net share PiProjects=D:\PiProjects /GRANT:trmcnealy,FULL



#sudo mkdir /mnt/Projects

#sudo apt-get install cifs-utils

#sudo mount -t auto //192.168.1.152/PiProjects /mnt/Projects -o rw,username=$USER

#pscp -i D:/pi-sh-key.ppk E:/Github/trmcnealy/IotDevices/RaspberryPiDevices.Tests/bin/ARM64/Debug/net8.0/linux-arm64/* trmcnealy@trmpi:/PiProjects



cd ~/Projects && rm --force --recursive *


#sudo git clone https://github.com/trmcnealy/IotDevices.git && sudo chmod 777 --recursive * && cd ~/Tests/IotDevices

#dotnet publish /p:NativeLib=Shared /p:UseCoreRT=true -r browser-wasm -f netstandard2.1 -c Release

#dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --configuration Debug --framework net8.0 --arch arm64 --os linux --self-contained false

#dotnet run --no-restore --project RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --property:Configuration=Debug

dotnet exec RaspberryPiDevices.Tests.dll

cd ~



#sudo chmod 777 --recursive *
#rm --force --recursive IotDevices

#ls -1 -lah

#sudo git clone https://github.com/trmcnealy/IotDevices.git
#sudo chmod 777 --recursive *
#sudo chmod 777 --recursive IotDevices

#sudo git status

#sudo git reset --hard
#sudo git clean -fd
#sudo git pull --rebase=true
#sudo git fetch
#sudo git rebase origin/main

#sudo git reset --hard && sudo git clean -fd && sudo git pull --rebase=true && sudo git fetch && sudo git rebase origin/main


# dotnet build RaspberryPiDevices/RaspberryPiDevices.csproj --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false --output ./trmcnealy/Tests/bin/
#dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --no-restore --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false


#cd ~/Tests/IotDevices/RaspberryPiDevices.Tests/bin/Debug/net8.0/linux-arm64/
#cd ~/Tests/IotDevices/

#dotnet restore

#dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --no-restore --configuration Debug --framework net8.0 --runtime linux-arm64 --self-contained false --use-current-runtime false
#dotnet build RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --configuration Debug --framework net8.0 --arch arm64 --os linux --self-contained false

#dotnet run --no-restore --project RaspberryPiDevices.Tests/RaspberryPiDevices.Tests.csproj --property:Configuration=Debug

#dotnet run RaspberryPiDevices.Tests.dll