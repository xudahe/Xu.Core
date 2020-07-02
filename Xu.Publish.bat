color B

del  .publishFiles\*.*   /s /q

dotnet restore

dotnet build

cd Xu.WebApi

dotnet publish -o ..\Xu.WebApi\bin\Debug\netcoreapp3.1\

md ..\.publishFiles

xcopy ..\Xu.WebApi\bin\Debug\netcoreapp3.1\*.* ..\.publishFiles\ /s /e 

echo "Successfully!!!! ^ please see the file .publishFiles"

cmd 