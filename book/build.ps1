 param (
    [string]$filter = "GalkovskyLJ_?.asc"
 )
 
 # This script builds all years.

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

$years = dir -path $scriptPath -file -filter "*$filter*"

ForEach($year in $years){
    Write-Host "===================="
    Write-Host $year.Name
    Write-Host "===================="
    Write-Host ""

    $bookFile = ($scriptPath + "\" + $year.Name)

    # Write-Host "Converting to HTML..."
    # & bundle exec asciidoctor $bookFile

    # Write-Host "Converting to EPub..."
    # & bundle exec asciidoctor-epub3 $bookFile

    # Write-Host "Converting to Mobi (kf8)..."
    # & bundle exec asciidoctor-epub3 -a ebook-format=kf8 $bookFile

    Write-Host "Converting to PDF..."
    & bundle exec asciidoctor-pdf $bookFile -a pdf-style=resources/themes/galkovsky-theme.yml
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
mv -Path ($scriptPath + "\*.epub") -Destination $outputPath -Force
mv -Path ($scriptPath + "\*.mobi") -Destination $outputPath -Force
rm ($scriptPath + "\*.pdfmarks")
mv -Path ($scriptPath + "\*.pdf") -Destination $outputPath -Force

Write-Host "Finished!"