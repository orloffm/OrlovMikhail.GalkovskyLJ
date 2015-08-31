 param (
    [string]$source =  "."
 )

# This script makes fragment files from sump files.
Write-Host ("Looking for all dumps in " + $source + "...")

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

$dumps = dir -recurse -path $source -file -filter dump.xml
Write-Host ("" + $dumps.Length + " found.")

ForEach($dump in $dumps){
	$fragmentPath = Join-Path $dump.Directory.FullName "fragment.asc"
	if(!(Test-Path -Path $fragmentPath)){
		$dumpPath = $dump.FullName
		Write-Host $dumpPath
		Start-Process -Wait -NoNewWindow -FilePath "$scriptPath\..\bin\bookmaker.exe" -ArgumentList "/root=`"$scriptPath`" /source=`"$dumpPath`""
	}
}