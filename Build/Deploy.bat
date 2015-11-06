@echo off
cls
Title Deploying DailySubsSubtitleDownloader
cd ..

if "%programfiles(x86)%XXX"=="XXX" goto 32BIT
	:: 64-bit
	set PROGS=%programfiles(x86)%
	goto CONT
:32BIT
	set PROGS=%ProgramFiles%	
:CONT

copy /y DailySubsSubtitleDownloader\bin\Release\DailySubsSubtitleDownloader.* "%PROGS%\Team MediaPortal\MediaPortal\SubtitleDownloaders\"

cd Build

pause