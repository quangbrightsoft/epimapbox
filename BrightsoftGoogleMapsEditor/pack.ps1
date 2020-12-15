$defaultVersion = "1.0.0"
$workingDirectory = Get-Location
$zip = "$workingDirectory\packages\7-Zip.CommandLine.18.1.0\tools\7za.exe"
$nuget = "$workingDirectory\nuget.exe"

function ZipCurrentModule {
    Param ([String]$moduleName)
    Robocopy.exe $defaultVersion\ $assemblyFileVersion\ /S
    Remove-Item "$moduleName.zip" -Force -Recurse -ErrorAction Ignore
    ((Get-Content -Path module.config -Raw).TrimEnd() -Replace $defaultVersion, $assemblyFileVersion ) | Set-Content -Path module.config
    Start-Process -NoNewWindow -Wait -FilePath $zip -ArgumentList "a", "$moduleName.zip", "$assemblyFileVersion", "module.config"
    ((Get-Content -Path module.config -Raw).TrimEnd() -Replace $assemblyFileVersion, $defaultVersion ) | Set-Content -Path module.config
    Remove-Item $assemblyFileVersion -Force -Recurse
}

$assemblyFileVersion = "1.0.9"

Write-Host "Creating nuget with $fileVersionMatch version and $assemblyFileVersion client assets version"

Set-Location src\modules\_protected\BrssMapsEditor
ZipCurrentModule -moduleName BrssMapsEditor
Set-Location $workingDirectory
Start-Process -NoNewWindow -Wait -FilePath $nuget -ArgumentList "pack", "$workingDirectory\BrightsoftGoogleMapsEditor.nuspec", "-Version $assemblyFileVersion", "-Properties configuration=Release", "-BasePath ./", "-Verbosity detailed"
