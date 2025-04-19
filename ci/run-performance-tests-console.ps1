param(
    [Parameter(Mandatory)][string]$RepoName,
    [Parameter(Mandatory)][string]$OrgName,
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$ExamplesRepo = "device-detection-dotnet-examples",
    [string]$ExamplesBranch = "main"
)
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 1.0

$resultsDir = New-Item -ItemType Directory -Force "$RepoName/test-results/performance-summary"
$tacFile = "$PWD/$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data/TAC-HashV41.hash"
$evidenceFile = "$PWD/$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data/20000 Evidence Records.yml"

Write-Host "Fetching examples..."
./steps/clone-repo.ps1 -RepoName $ExamplesRepo -OrgName $OrgName -Branch $ExamplesBranch
# Shorter name for Windows compatibility. -PassThru parameter doesn't work,
# 'Name' property is empty for some reason
Rename-Item $ExamplesRepo "ex"
$ExamplesRepo = "ex"

$exampleBase = "$ExamplesRepo/Examples/ExampleBase"
$ddProject = "$PWD/$RepoName/FiftyOne.DeviceDetection/FiftyOne.DeviceDetection.csproj"
Push-Location $exampleBase
try {
    Write-Output "Replacing the DeviceDetection package with a local reference in $exampleBase"
    dotnet remove package "FiftyOne.DeviceDetection"
    dotnet add reference $ddProject
} finally {
    Pop-Location
}

Push-Location $ExamplesRepo/Examples/OnPremise/Performance-Console
try {
    Write-Host "Running performance example with config $Configuration|$Arch"
    dotnet build -c $Configuration /p:Platform=$Arch /p:OutDir=output /p:BuiltOnCI=true
    dotnet output/FiftyOne.DeviceDetection.Examples.OnPremise.Performance.dll -d $tacFile -u $evidenceFile -j summary.json
    
    # Write out the results for comparison
    Write-Host "Writing performance test results"
    $results = Get-Content summary.json | ConvertFrom-Json
    Write-Output "{
        'HigherIsBetter': {
            'DetectionsPerSecond': $($results.MaxPerformance.DetectionsPerSecond)
        },
        'LowerIsBetter': {
            'MsPerDetection': $($results.MaxPerformance.MsPerDetection)
        }
    }" > "$resultsDir/results_$Name.json"
} finally {
    Pop-Location
}
