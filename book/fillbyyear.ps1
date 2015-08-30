# This script fills per-year inclusion files.

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

$years = dir -path $scriptPath -directory -filter 2???

$utf8 = New-Object System.Text.UTF8Encoding($true)

ForEach($year in $years){
    Write-Host $year.Name
    $posts = dir -directory -path $year.FullName -filter ????

    $yearSubPath = $year.Name + "\fullyear.asc"
    $stream = New-Object System.IO.StreamWriter(($scriptPath + "\" + $yearSubPath), $false, $utf8) 

    ForEach($post in $posts){
        If($post.GetFiles("fragment.asc").Length -eq 1){
            Write-Host (" " + $post.Name)
		    $stream.WriteLine("include::" + $post.Name + "\fragment.asc[]")
		    $stream.WriteLine("")
        }
    }

    $stream.Close()

    # If doesn't exist, create the book root for this year.
    $rootPath = $scriptPath + "\Galkovsky_LJ_" + $year.Name + ".asc"
    $rootExists = Test-Path $rootPath
    If (-Not $rootExists){
        $rootStream = New-Object System.IO.StreamWriter($rootPath, $false, $utf8) 

        # Header
        $title = ("ЖЖ Галковского за " + $year.Name + " год")
        $rootStream.WriteLine($title)
        $underline = New-Object String '=', $title.Length
        $rootStream.WriteLine($underline)
        $rootStream.WriteLine(":doctype:   book")
        $rootStream.WriteLine(":docinfo:")
        $rootStream.WriteLine(":toc:")
        $rootStream.WriteLine(":toclevels: 2")
        $rootStream.WriteLine("")
        $rootStream.WriteLine("include::intro.asc[]")
        $rootStream.WriteLine("")
        $rootStream.WriteLine("include::" + $yearSubPath + "[]")
        $rootStream.Close()
    }
}