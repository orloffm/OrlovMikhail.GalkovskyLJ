 param (
    [string]$source = "."
 )
 
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

"Removing all fragment.asc files from $source..."

dir -path $source -recurse -include "fragment.asc" | %{ rm $_.FullName }