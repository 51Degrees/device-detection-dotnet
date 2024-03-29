param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$OrgName,
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64"
)

$RepoPath = [IO.Path]::Combine($pwd, $RepoName)
$PerfResultsFile = [IO.Path]::Combine($RepoPath, "test-results", "performance-summary", "results_$Name.json")
$EvidenceFiles = [IO.Path]::Combine($pwd, $RepoName,"FiftyOne.DeviceDetection.Hash.Engine.OnPremise", "device-detection-cxx", "device-detection-data")
$ExamplesRepoName = "device-detection-dotnet-examples"
$ExamplesRepoPath = [IO.Path]::Combine($pwd, $ExamplesRepoName)
$DeviceDetectionProject = [IO.Path]::Combine($RepoPath, "FiftyOne.DeviceDetection", "FiftyOne.DeviceDetection.csproj")
$PerfProject = [IO.Path]::Combine($ExamplesRepoPath, "Examples", "OnPremise", "Performance-Console")

Write-Output "Entering '$RepoPath'"
Push-Location $RepoPath

try {

    # Create the output directories if they don't already exist.
    if ($(Test-Path -Path "test-results") -eq  $False) {
        mkdir test-results
    }
    if ($(Test-Path -Path "test-results/performance-summary") -eq  $False) {
        mkdir test-results/performance-summary
    }

}
finally {

    Write-Output "Leaving '$RepoPath'"
    Pop-Location

}

try {
    if ($(Test-Path -Path $ExamplesRepoName) -eq $False) {
        Write-Output "Cloning '$ExamplesRepoName'"
        ./steps/clone-repo.ps1 -RepoName $ExamplesRepoName -OrgName $OrgName
    }

    Write-Output "Moving TAC file"
    $TacFile = [IO.Path]::Combine($EvidenceFiles, "TAC-HashV41.hash") 
    Copy-Item $TacFile device-detection-dotnet-examples/device-detection-data/TAC-HashV41.hash

    Write-Output "Moving evidence file"
    $EvidenceFile = [IO.Path]::Combine($EvidenceFiles, "20000 Evidence Records.yml")
    Copy-Item $EvidenceFile "device-detection-dotnet-examples/device-detection-data/20000 Evidence Records.yml"
    
    $ExamplesProject = [IO.Path]::Combine($ExamplesRepoPath, "Examples", "ExampleBase")
    
    # Update the dependency in the examples project to point to the newly bulit package
    Write-Output "Entering '$ExamplesProject'"
    Push-Location $ExamplesProject
    try{
        # Change the dependency version to the locally build Nuget package
        Write-Output "Replacing the DeviceDetection package with a local reference."
        dotnet remove package "FiftyOne.DeviceDetection"
        dotnet add reference $DeviceDetectionProject
    }
    finally{
        Write-Output "Leaving '$ExamplesProject'"
        Pop-Location
    }
    
    Write-Output "Running performance example with config $Configuration|$Arch"
    Write-Output "Entering '$PerfProject' folder"
    Push-Location "$PerfProject"
    try {
        $RunConfig = "Debug"
        if ($Configuration.Contains("Release")) {
            $RunConfig = "Release"
        }
        dotnet build -c $RunConfig /p:Platform=$Arch /p:OutDir=output /p:BuiltOnCI=true
        Push-Location "output"
        try {
            dotnet FiftyOne.DeviceDetection.Examples.OnPremise.Performance.dll -d $TacFile -u $EvidenceFile -j summary.json
        }
        finally {
            Pop-Location
        }
        
        if ($LASTEXITCODE -ne 0) {
            exit $LASTEXITCODE
        }

        # Write out the results for comparison
        Write-Output "Writing performance test results"
        $Results = Get-Content ./output/summary.json | ConvertFrom-Json
        Write-Output "{
            'HigherIsBetter': {
                'DetectionsPerSecond': $($Results.MaxPerformance.DetectionsPerSecond)
            },
            'LowerIsBetter': {
                'MsPerDetection': $($Results.MaxPerformance.MsPerDetection)
            }
        }" > $PerfResultsFile

    }
    finally {
        Write-Output "Leaving '$PerfProject'"
        Pop-Location
    }

    Copy-Item $ExamplesRepoName/test-results $RepoName -Recurse
}

finally {
    exit $LASTEXITCODE
}
