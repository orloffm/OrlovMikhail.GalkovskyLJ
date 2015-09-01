@ECHO OFF

SET man1=%1

IF "%1"=="" (
   SET man1=.
)

PowerShell.exe -NoProfile -ExecutionPolicy Bypass -Command "& '%~dpn0.ps1' -source %man1%"