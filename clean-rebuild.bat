@echo off
echo Cleaning project directories...
if exist "Quiz\bin" rmdir /s /q "Quiz\bin"
if exist "Quiz\obj" rmdir /s /q "Quiz\obj"

echo Restoring NuGet packages...
nuget restore

echo Rebuilding the project...
msbuild /t:Rebuild /p:Configuration=Debug

echo Done!
pause 