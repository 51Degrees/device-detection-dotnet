
param(
    [string]$ProjectDir = ".",
    [string]$Name,
    [Parameter(Mandatory=$true)]
    [string]$RepoName
)

./dotnet/run-update-dependencies.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name

exit $LASTEXITCODE