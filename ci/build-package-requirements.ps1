param(
    [string]$ProjectDir = "FiftyOne.DeviceDetection",
    [string]$Name = "Release_x64",
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$Arch
)

if($IsLinux){
    $Archs = @("x86", "x64")
    foreach($a in $Archs){
        ./dotnet/build-package-requirement.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration "Release" -Arch $a
    }
}
else{
    ./dotnet/build-package-requirement.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration "Release" -Arch $Arch
}
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

$NativeFilesFolder = [IO.Path]::Combine($RepoPath, $ProjectDir, $Subfolder) 

$PackageFolder = "package-files"
New-Item -Path $PackageFolder -ItemType Directory -Force


Copy-Item -Path "$NativeFilesFolder" -Destination "$PackageFolder" -Recurse -Force


exit $LASTEXITCODE
