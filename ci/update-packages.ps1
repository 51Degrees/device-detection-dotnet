param(
    [Parameter(Mandatory)][string]$RepoName,
    [string]$Name
)

./dotnet/run-update-dependencies.ps1 -RepoName $RepoName -Name $Name
