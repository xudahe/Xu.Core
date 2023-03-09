color B

del  .publishFiles\*.*   /s /q

dotnet restore

dotnet build

cd Xu.WebApi

dotnet publish -o ..\Xu.WebApi\bin\Debug\net6.0\

md ..\.publishFiles

xcopy ..\Xu.WebApi\bin\Debug\net6.0\*.* ..\.publishFiles\ /s /e 

echo "Successfully!!!! ^ please see the file .publishFiles"

cmd 