cls
SET srcpath=%~dp0
echo %mypath%

cd "./06-Frontends/Silo/"
start "" dotnet run

chdir /d %srcpath%
cd "./06-Frontends/WebApi/"
start "" dotnet run

chdir /d %srcpath%
cd "./06-Frontends/WebClient/dashboard/"
call ng-serve.bat
