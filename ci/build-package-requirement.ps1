param(
    [string]$ProjectDir = "FiftyOne.DeviceDetection",
    [string]$Name = "Release_x64",
    [string]$RepoName
)

./dotnet/build-package-requirement.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration "Release"

$RepoPath = [IO.Path]::Combine($pwd, $RepoName)

if (Test-Path -Path "$RepoPath\$ProjectDir\windows") {
    $Subfolder = "windows"
}
elseif (Test-Path -Path "$RepoPath\$ProjectDir\linux") {
    $Subfolder = "linux"
}
else {
    Write-Host "No appropriate subfolder found."
    exit
}

$Files = Get-ChildItem -Path "$RepoPath/$ProjectDir/$Subfolder" -Include "*.dll", "*.so" -Recurse -File

$PackageFolder = "package-files"
New-Item -Path $PackageFolder -ItemType Directory -Force

foreach ($file in $Files) {
    Copy-Item -Path $file -Destination "$PackageFolder/$($file.Name)"
}

exit $LASTEXITCODE
