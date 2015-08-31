SET man1=%1

IF "%1"=="" (
   SET man1=Galkovsky_?.asc
)

PowerShell.exe -NoProfile -ExecutionPolicy Bypass -Command "& '%~dpn0.ps1' -source %1"