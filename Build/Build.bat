@echo off
cls
Title Building DailySubsSubtitleDownloader
cd ..

"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /target:Rebuild /property:Configuration=RELEASE DailySubsSubtitleDownloader.sln

cd Build

pause