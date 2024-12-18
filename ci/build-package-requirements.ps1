param(
    [string]$ProjectDir = "FiftyOne.DeviceDetection.Hash.Engine.OnPremise",
    [string]$Name = "Release_x64",
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$Arch
)
$RepoPath = [IO.Path]::Combine($pwd, $RepoName)

$NativeName = "FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll"
$NativeFile = [IO.Path]::Combine($RepoPath, $ProjectDir, "build", "Release", $NativeName)

if($IsLinux){
    $Subfolder = "linux"
    # Because .NET does not officially support the x86 architecture on Linux distributions, we are using the x64 job to build x86 binaries.
    $Archs = @("x86", "x64")
    foreach($a in $Archs){
        ./dotnet/build-package-requirement.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration "Release" -Arch $a
        $PackageFolder = "package-files/$SubFolder/$a"
        New-Item -Path $PackageFolder -ItemType Directory -Force
        Copy-Item -Path $NativeFile -Destination "$PackageFolder/$NativeName" -Force
      
        # CMake generates build files specific to the architecture. We are deleting the build folder to ensure clean build enviroment. 
        Remove-Item -LiteralPath "$RepoPath/$ProjectDir/build" -Force -Recurse -ErrorAction SilentlyContinue
    }
}
elseif ($IsWindows) {
    $Subfolder = "windows"
    ./dotnet/build-package-requirement.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration "Release" -Arch $Arch
    $PackageFolder = "package-files/$SubFolder/$Arch"
    New-Item -Path $PackageFolder -ItemType Directory -Force
    Copy-Item -Path $NativeFile -Destination "$PackageFolder/$NativeName" -Force
}
elseif ($IsMacOS) {
    $Subfolder = "macos"
    ./cxx/build-project.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Configuration "Release" -Arch $Arch
    $PackageFolder = "package-files/$SubFolder/$Arch"
    New-Item -Path $PackageFolder -ItemType Directory -Force
    Copy-Item -Path $NativeFile -Destination "$PackageFolder/$NativeName" -Force
    
    # CMake generates build files specific to the architecture. We are deleting the build folder to ensure clean build enviroment. 
    Remove-Item -LiteralPath "$RepoPath/$ProjectDir/build" -Force -Recurse -ErrorAction SilentlyContinue
}
else {
    Write-Host "Unsupported OS."
    exit
}



exit $LASTEXITCODE
