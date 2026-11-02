param(
    [Parameter(Mandatory)][string]$RepoName,
    [string]$OrgName,
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$ExamplesBranch = "main"
)

& $RepoName/ci/run-performance-tests-console.ps1 -RepoName:$RepoName -OrgName:$OrgName -Name:$Name -Configuration:$Configuration -Arch:$Arch -ExamplesBranch:$ExamplesBranch
& $RepoName/ci/run-performance-tests-web.ps1 -Name:$Name -Configuration:$Configuration -Arch:$Arch
