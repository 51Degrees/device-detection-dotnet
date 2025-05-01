param(
    [Parameter(Mandatory)][string]$RepoName,
    [Parameter(Mandatory)][string]$OrgName,
    [string]$BuildMethod = "dotnet",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$ExamplesRepo = "device-detection-dotnet-examples",
    [string]$ExamplesBranch = "version/4.5",
    [string]$Version
)
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 1.0

# If Version is not provided, the script is running in a workflow that doesn't
# build packages and the integration tests will be skipped
if (!$Version) {
    Write-Host "Skipping integration tests"
    exit 0
}

Write-Host "Fetching examples..."
./steps/clone-repo.ps1 -RepoName $ExamplesRepo -OrgName $OrgName -Branch $ExamplesBranch
# Shorter name for Windows compatibility. -PassThru parameter doesn't work,
# 'Name' property is empty for some reason
Rename-Item $ExamplesRepo "ex"
$ExamplesRepo = "ex"

Write-Host "Moving TAC and evidence files for examples..."
foreach ($_ in "20000 User Agents.csv", "20000 Evidence Records.yml", "51Degrees-LiteV4.1.hash", "TAC-HashV41.hash") {
    Write-Host "Linking $_"
    New-Item -ItemType SymbolicLink -Force -Target "$PWD/$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data/$_" -Path "$ExamplesRepo/device-detection-data/$_"
}

# Install the nuget packages to the local feed. The packages in the 'package'
# folder must be pushed to local feed and cannot be used directly, as all the
# other dependencies will be installed in the local feed.
$localFeed = New-Item -ItemType Directory -Force "$HOME/.nuget/packages"
dotnet nuget add source $localFeed
dotnet nuget push -s $localFeed (Get-ChildItem -Path package -Filter '*.nupkg')

Write-Host "Restoring Examples Project..."
dotnet restore $ExamplesRepo
Get-ChildItem -Path $ExamplesRepo -Recurse -File -Filter '*.csproj' | ForEach-Object {
    Write-Host "Setting DeviceDetection version for '$_' to $Version"
    dotnet add $_ package "FiftyOne.DeviceDetection" --version $Version
}

Write-Host "Building Examples Project..."
& $ExamplesRepo/ci/build-project.ps1 -RepoName $ExamplesRepo -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod
Write-Host "Testing Examples Project..."
./dotnet/run-integration-tests.ps1 -RepoName $ExamplesRepo -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod -DirNameFormatForDotnet "*" -DirNameFormatForNotDotnet "*" -Filter ".*\.sln"

Copy-Item $ExamplesRepo/test-results $RepoName -Recurse
