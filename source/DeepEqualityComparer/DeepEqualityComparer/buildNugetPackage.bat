@echo off
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" DeepEqualityComparer.csproj /p:Configuration=Release
nuget pack DeepEqualityComparer.csproj -Prop configuration=Release
