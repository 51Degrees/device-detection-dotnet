param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "CoreRelease",
    [string]$Arch = "x64",
    [string]$Version,
    [string]$BuildMethod,
    [string]$OrgName
)

$NugetPackageFolder = [IO.Path]::Combine($pwd, "package")
$EvidenceFiles = [IO.Path]::Combine($pwd, $RepoName,"FiftyOne.DeviceDetection", "device-detection-cxx", "device-detection-data")
$ExamplesRepoName = "device-detection-dotnet-examples"
$ExamplesRepoPath = [IO.Path]::Combine($pwd, $ExamplesRepoName)

try {
    # If the $Version is empty it means that this script is running in a workflow that will not build packages and integration tests will be skipped.
    if([String]::IsNullOrEmpty($Version) -eq $False) { 
        Write-Output "Cloning '$ExamplesRepoName'"
        ./steps/clone-repo.ps1 -RepoName $ExamplesRepoName -OrgName $OrgName
        
        Write-Output "Moving TAC file for examples"
        $TacFile = [IO.Path]::Combine($EvidenceFiles, "TAC-HashV41.hash") 
        Copy-Item $TacFile device-detection-dotnet-examples/device-detection-data/TAC-HashV41.hash

        Write-Output "Moving evidence files for examples"
        $UAFile = [IO.Path]::Combine($EvidenceFiles, "20000 User Agents.csv") 
        $EvidenceFile = [IO.Path]::Combine($EvidenceFiles, "20000 Evidence Records.yml")
        Copy-Item $UAFile "device-detection-dotnet-examples/device-detection-data/20000 User Agents.csv"
        Copy-Item $EvidenceFile "device-detection-dotnet-examples/device-detection-data/20000 Evidence Records.yml"
        
        $ExamplesProject = [IO.Path]::Combine($ExamplesRepoPath, "Examples", "ExampleBase")
        
        # Restore nuget packages in the examples project
        try {
            Write-Output "Entering '$ExamplesRepoPath'"
            Push-Location $ExamplesRepoPath

            Write-Output "Running Nuget Restore"
            nuget restore
        }
        finally {

            Write-Output "Leaving '$ExamplesRepoPath'"
            Pop-Location
        }
        
        $LocalFeed = [IO.Path]::Combine($Home, ".nuget", "packages")
        
        # Install the nuget packages to the local feed. 
        # The packages in the 'package' folder must be pushed to local feed and cannot be used directly,
        # as all the other dependencies will already be installed in the local feed.
        try{
            Write-Output "Entering '$NugetPackageFolder' folder"
            Push-Location "$NugetPackageFolder"
            
            Write-Output "Pushing nuget packages to the local feed"
            dotnet nuget push "*.nupkg" -s "$LocalFeed"
        }
        finally{
            Write-Output "Leaving '$NugetPackageFolder'"
            Pop-Location
        }
        
        # Update the dependency in the examples project to point to the newly bulit package
        try{
            Write-Output "Entering '$ExamplesProject'"
            Push-Location $ExamplesProject

            # Change the dependency version to the locally build Nuget package
            Write-Output "Setting the version of the DeviceDetection package to '$Version'"
            dotnet add package "FiftyOne.DeviceDetection" --version $Version --source "$LocalFeed" 
        }
        finally{
            Write-Output "Leaving '$ExamplesProject'"
            Pop-Location
        }
        
        # Build and Test Examples project now that all is set up
        Write-Output "Building project with following configuration '$Configuration|$Arch|$BuildMethod'"
        ./device-detection-dotnet-examples/ci/build-project.ps1 -RepoName $ExamplesRepoName -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod
        
        Write-Output "Testing Examples Project"
        ./dotnet/run-integration-tests.ps1 -RepoName $ExamplesRepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod -Filter ".*Tests(|\.Web)\.dll"
    } 
    else{
        Write-Output "Not running integration tests at this stage."
    }
}

finally {
    exit $LASTEXITCODE
}

