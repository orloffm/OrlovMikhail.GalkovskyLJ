 param (
    [string]$filter = "Galkovsky_?.asc"
 )
 
 # This script builds all years.

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
cd $scriptPath

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

$outputPath = ($scriptPath + "\output")
if((Test-Path $outputPath) -eq 0) {
        mkdir $outputPath
}
else{
    Remove-Item ($outputPath + "\*") -Recurse
}


Write-Host "Moving files to output folder..."
Move-Item -Path ($scriptPath + "\*.html") -Destination $outputPath
Move-Item -Path ($scriptPath + "\*.epub") -Destination $outputPath
Move-Item -Path ($scriptPath + "\*.mobi") -Destination $outputPath
Remove-Item ($scriptPath + "\*.pdfmarks")
Move-Item -Path ($scriptPath + "\*.pdf") -Destination $outputPath

Write-Host "Finished!"