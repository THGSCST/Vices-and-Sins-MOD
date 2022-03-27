:Files to delete
set FileList=EULA.txt GameConfiguration.dll GameuxInstallHelper.dll gfw_high.ico gog.ico goggame.dll GOGLauncher.exe innosetup_license.txt itn.ico "Launch Gangsters.lnk" Launcher.exe libgcc_s_dw2-.dll ingwm10.dll MplayerReadme.wri mplaynow.exe mplaynow.ini QtCore4.dll Security.key Support.ico

@echo off
cls
title Cleanup tool for Gangsters Organized Crime GOG
echo This tool will delete all unnecessary files created during the installation of Gangsters Organized Crime GOG version.
@pause

echo.
if exist gangsters.exe goto:START
echo Error: GANGSTERS.EXE was not found in this folder!
echo Do you know game's installation directory? Make sure you copied the tool to the right folder.
pause >nul
exit

:START
FOR %%A IN (%FileList%) DO (
    if exist echo File deleted: %%A
    DEL %%A
)
echo.
echo Done!
@pause
goto 2> NUL & Del "%~f0"
exit