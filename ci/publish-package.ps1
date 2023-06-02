
param(
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [Parameter(Mandatory=$true)]
    [string]$ApiKey
)

#./dotnet/publish-package-nuget.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -ApiKey $ApiKey


exit $LASTEXITCODE
