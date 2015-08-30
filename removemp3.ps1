$files = dir C:\dev\galkovskylj\book -recurse -include "mappings.txt"

function ToArray
{
  begin
  {
    $output = @();
  }
  process
  {
    $output += $_;
  }
  end
  {
    return ,$output;
  }
}

foreach($file in $files){
    Write-Host $file.FullName
    $lines = [System.IO.File]::ReadAllLines($file.FullName)

    $filteredLines = $lines | Where {$_.Contains('.mp3') -eq $False} | ToArray

    [System.IO.File]::WriteAllLines($file.FullName, $filteredLines)
}