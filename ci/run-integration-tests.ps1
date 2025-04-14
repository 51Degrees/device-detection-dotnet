param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$Version,
    [string]$BuildMethod,
    [string]$OrgName
)

$NugetPackageFolder = [IO.Path]::Combine($pwd, "package")
$EvidenceFilesSrc = [IO.Path]::Combine($pwd, $RepoName,"FiftyOne.DeviceDetection.Hash.Engine.OnPremise", "device-detection-cxx", "device-detection-data")
$ExamplesRepoName = "device-detection-dotnet-examples"
$ExamplesRepoPath = [IO.Path]::Combine($pwd, $ExamplesRepoName)
$EvidenceFilesDst = [IO.Path]::Combine($ExamplesRepoPath, "device-detection-data")

# If the $Version is empty it means that this script is running in a workflow that will not build packages and integration tests will be skipped.
if([String]::IsNullOrEmpty($Version) -eq $False) { 
    Write-Output "Cloning '$ExamplesRepoName'"
    ./steps/clone-repo.ps1 -RepoName $ExamplesRepoName -OrgName $OrgName

    Write-Output "Moving TAC and evidence files for examples..."
    $EvidenceFileNames = "20000 User Agents.csv", "20000 Evidence Records.yml", "51Degrees-LiteV4.1.hash", "TAC-HashV41.hash"
    foreach ($NextEvidenceFile in $EvidenceFileNames)
    {
        Write-Output "  - Copying $NextEvidenceFile..."
        $NextEvidenceFileSrc = [IO.Path]::Combine($EvidenceFilesSrc, $NextEvidenceFile)
        $NextEvidenceFileDst = [IO.Path]::Combine($EvidenceFilesDst, $NextEvidenceFile)
        Copy-Item $NextEvidenceFileSrc $NextEvidenceFileDst
    }
    
    # Restore nuget packages in the examples project
    try {
        Write-Output "Entering '$ExamplesRepoPath'"
        Push-Location $ExamplesRepoPath

        Write-Output "Running Nuget Restore"
        dotnet nuget list source
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
        dotnet nuget add source "$LocalFeed"
        dotnet nuget push "*.nupkg" -s "$LocalFeed"
    }
    finally{
        Write-Output "Leaving '$NugetPackageFolder'"
        Pop-Location
    }
    
    # Update the dependency in the examples project to point to the newly bulit package
    try{
        Write-Output "Entering '$ExamplesRepoPath'"
        Push-Location $ExamplesRepoPath

        Write-Output ""
        Write-Output "Listing csproj files to update..."
        Get-ChildItem -Path $ExamplesRepoPath -Recurse -File -Filter "*.csproj" -Name
        Write-Output ""

        # Change the dependency version to the locally build Nuget package
        Write-Output "Setting the version of the DeviceDetection package to '$Version'..."
        Get-ChildItem -Path $ExamplesRepoPath -Recurse -File -Filter "*.csproj" | ForEach-Object {
            $NextFullName = $_.FullName
            Write-Output ""
            Write-Output "Will set the version of the DeviceDetection package to '$Version' in $NextFullName..."
            dotnet add $NextFullName package "FiftyOne.DeviceDetection" --version $Version
            Write-Output "Did set the version of the DeviceDetection package to '$Version' in $NextFullName..."
            Write-Output ""
        }
        Write-Output ""
    }
    finally{
        Write-Output "Leaving '$ExamplesRepoPath'"
        Pop-Location
    }
    
    # Test Examples project now that all is set up
    Write-Output "Building Examples Project"
    & $ExamplesRepoPath/ci/build-project.ps1 -RepoName "device-detection-dotnet-examples" -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod
    Write-Output "Testing Examples Project"
    ./dotnet/run-integration-tests.ps1 -RepoName $ExamplesRepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod "dotnet" -DirNameFormatForDotnet "*" -DirNameFormatForNotDotnet "*" -Filter ".*\.sln"
    
    Copy-Item $ExamplesRepoName/test-results $RepoName -Recurse
} 
else{
    Write-Output "Not running integration tests at this stage."
}
