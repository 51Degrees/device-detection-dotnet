param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$OrgName,
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64"
)

. ./run-performance-tests-console.ps1 -RepoName $RepoName -OrgName $OrgName -Name $Name -Configuration $Configuration -Arch $Arch
. ./run-performance-tests-web.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch