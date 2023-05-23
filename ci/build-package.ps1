param(
    [string]$ProjectDir = ".",
    [string]$RepoName,
    [string]$Name = "Release_x64",
    [string]$Configuration = "CoreRelease",
    [string]$Arch = "x64",
    [Parameter(Mandatory=$true)]
    [string]$Version
)

./dotnet/build-package-nuget.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -Version $Version -SolutionName "FiftyOne.DeviceDetection"

exit $LASTEXITCODE