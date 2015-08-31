 param (
    [string]$source = "."
 )
 
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

dir -path $source -recurse -include "fragment.asc" | %{ rm $_.FullName }