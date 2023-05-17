param(
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "CoreRelease",
    [string]$Arch = "x64",
    [Parameter(Mandatory=$true)]
    [string]$Version
)

./dotnet/build-package-nuget.ps1 -RepoName "device-detection-dotnet" -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -Version $Version

exit $LASTEXITCODE