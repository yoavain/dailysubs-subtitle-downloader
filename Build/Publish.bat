@echo off
cls
Title Deploying DailySubsSubtitleDownloader
cd ..

copy /y SubsCenterOrg\bin\Release\SubsCenterOrg.dll "..\subcentral\External\SubtitleDownloaders\"
copy /y SubsCenterOrg\bin\Release\SubsCenterOrg.xml "..\subcentral\External\SubtitleDownloaders\"
copy /y Sratim\bin\Release\Sratim.dll "..\subcentral\External\SubtitleDownloaders\"
copy /y Sratim\bin\Release\Sratim.xml "..\subcentral\External\SubtitleDownloaders\"

explorer "..\subcentral\External\SubtitleDownloaders\"

cd Build