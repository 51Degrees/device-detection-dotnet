param(
    [string]$ProjectDir = "FiftyOne.DeviceDetection.Hash.Engine.OnPremise",
    [string]$Name = "Release_x64",
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$Arch
)
$ErrorActionPreference = "Stop"

$RepoPath = [IO.Path]::Combine($pwd, $RepoName)

$NativeName = "FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll"
$NativeFile = [IO.Path]::Combine($RepoPath, $ProjectDir, "build", "Release", $NativeName)

if ($IsLinux) {
    $Subfolder = "linux"
    ./dotnet/build-package-requirement.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration "Release" -Arch $Arch
    $PackageFolder = "package-files/$SubFolder/$Arch"
    New-Item -Path $PackageFolder -ItemType Directory -Force
    Copy-Item -Path $NativeFile -Destination "$PackageFolder/$NativeName" -Force
    # CMake generates build files specific to the architecture. We are deleting the build folder to ensure clean build enviroment. 
    Remove-Item -LiteralPath "$RepoPath/$ProjectDir/build" -Force -Recurse -ErrorAction SilentlyContinue
} elseif ($IsWindows) {
    $Subfolder = "windows"
    ./dotnet/build-package-requirement.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration "Release" -Arch $Arch
    $PackageFolder = "package-files/$SubFolder/$Arch"
    New-Item -Path $PackageFolder -ItemType Directory -Force
    Copy-Item -Path $NativeFile -Destination "$PackageFolder/$NativeName" -Force
} elseif ($IsMacOS) {
    $Subfolder = "macos"
    ./cxx/build-project.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Configuration "Release" -Arch $Arch
    $PackageFolder = "package-files/$SubFolder/$Arch"
    New-Item -Path $PackageFolder -ItemType Directory -Force
    Copy-Item -Path $NativeFile -Destination "$PackageFolder/$NativeName" -Force
    # CMake generates build files specific to the architecture. We are deleting the build folder to ensure clean build enviroment. 
    Remove-Item -LiteralPath "$RepoPath/$ProjectDir/build" -Force -Recurse -ErrorAction SilentlyContinue
} else {
    Write-Host "Unsupported OS."
    exit 1
}
