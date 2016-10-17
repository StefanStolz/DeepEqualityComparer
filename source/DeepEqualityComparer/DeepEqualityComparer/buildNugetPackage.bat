@echo off
msbuild DeepEqualityComparer.csproj /p:Configuration=Release
nuget pack DeepEqualityComparer.csproj -Prop configuration=Release
