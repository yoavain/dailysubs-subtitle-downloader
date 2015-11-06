@echo off
cls
Title Building Addic7edSubtitleDownloader [DEBUG]
cd ..

"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /target:Rebuild /property:Configuration=DEBUG Addic7edSubtitleDownloader.sln

cd Build

pause