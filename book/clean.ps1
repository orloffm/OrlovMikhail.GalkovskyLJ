 param (
    [string]$source = "."
 )
 
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

IF($source -eq "."){
	"Removing all .asc and output files..."
	dir -path $scriptPath -recurse -filter "*.asc" | %{ rm $_.FullName }
	dir -path $scriptPath -recurse -filter "*.pdf" | %{ rm $_.FullName }
}
else{
	"Removing all fragment.asc files from $source..."
	dir -path $source -recurse -include "fragment.asc" | %{ rm $_.FullName }
}