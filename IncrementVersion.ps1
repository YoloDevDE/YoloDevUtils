$projectFile = "YoloDev.csproj"
if (Test-Path $projectFile) {
    [xml]$xml = Get-Content $projectFile
    $versionNode = $xml.SelectSingleNode("//Version")
    if ($versionNode -ne $null) {
        $version = [version]$versionNode.InnerText
        $newVersion = New-Object System.Version($version.Major, $version.Minor, ($version.Build + 1))
        $versionNode.InnerText = $newVersion.ToString()
        $xml.Save($projectFile)
        Write-Host "Version increased to $newVersion in $projectFile"
    } else {
        Write-Error "Version node not found in $projectFile"
    }
} else {
    Write-Error "Project file $projectFile not found"
}
