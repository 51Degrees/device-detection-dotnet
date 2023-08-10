param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName = "device-detection-dotnet",
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$BuildMethod = "msbuild"
)

./dotnet/build-package-requirement.ps1 -RepoName $RepoName -ProjectDir "FiftyOne.DeviceDetection.Hash.OnPremise" -Name $Name -Configuration "Release" -Arch $Arch

if ($BuildMethod -eq "dotnet"){

    ./dotnet/build-project-core.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch

}
else{

    ./dotnet/build-project-framework.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch
}

exit $LASTEXITCODE