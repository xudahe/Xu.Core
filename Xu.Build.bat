@echo off
for /f "tokens=5" %%i in ('netstat -aon ^| findstr ":8091"') do (
    set n=%%i
)
taskkill /f /pid %n%




dotnet build

cd Xu.WebApi



dotnet run

cmd