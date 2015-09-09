 param (
    [string]$filter = "GalkovskyLJ_?.asc"
 )
 
 # This script builds all years.

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

$years = dir -path $scriptPath -filter "*$filter*"

$env:KINDLEGEN =  Join-Path $scriptPath 'resources\kindlegen.exe'

ForEach($year in $years){
	If([System.IO.Path]::GetExtension($year) -ne ".asc"){
		continue
	}

    " "
    "===================="
    $year.Name
    "===================="
    " "

    $bookFile = ($scriptPath + "\" + $year.Name)

    # "Converting to HTML..."
    # & bundle exec asciidoctor $bookFile

    "Converting to EPub..."
    & bundle exec asciidoctor-epub3 $bookFile

    # "Converting to Mobi (kf8)..."
    # & bundle exec asciidoctor-epub3 -a ebook-format=kf8 $bookFile

    "Converting to PDF..."
    & bundle exec asciidoctor-pdf $bookFile -a pdf-style=resources/themes/galkovsky-theme.yml --trace
}

$outputPath = Join-Path $scriptPath "output"
if(!(Test-Path -Path $outputPath)) {
	mkdir $outputPath
}
else{
    #Remv ($outputPath + "\*") -Recurse
}


Write-Host "Moving files to output folder..."
mv -Path ($scriptPath + "\*.html") -Destination $outputPath -Force
rm ($scriptPath + "\*-kf8.epub")
mv -Path ($scriptPath + "\*.epub") -Destination $outputPath -Force
mv -Path ($scriptPath + "\*.mobi") -Destination $outputPath -Force
rm ($scriptPath + "\*.pdfmarks")
mv -Path ($scriptPath + "\*.pdf") -Destination $outputPath -Force

Write-Host "Finished!"