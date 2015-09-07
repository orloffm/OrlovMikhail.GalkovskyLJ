@ECHO OFF

SET man1=%1

IF "%1"=="" (
   SET man1=%~dp0
)

%~dp0\..\bin\bookmaker.exe /root=%~dp0 /source=%man1%