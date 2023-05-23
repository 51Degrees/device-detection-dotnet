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
    Write-Output "Cloning '$ExamplesRepoName'"
    # ./steps/clone-repo.ps1 -RepoName "device-detection-dotnet-examples" -OrgName $OrgName
    
    Write-Output "Moving TAC file for examples"
    $TacFile = [IO.Path]::Combine($EvidenceFiles, "TAC-HashV41.hash") 
    Copy-Item $TacFile device-detection-dotnet-examples/device-detection-data/TAC-HashV41.hash

   # $LiteFile = [IO.Path]::Combine($EvidenceFiles, "51Degrees-LiteV4.1.hash") 

    Write-Output "Moving evidence files for examples"
    $UAFile = [IO.Path]::Combine($EvidenceFiles, "20000 User Agents.csv") 
    $EvidenceFile = [IO.Path]::Combine($EvidenceFiles, "20000 Evidence Records.yml")
    Copy-Item $UAFile "device-detection-dotnet-examples/device-detection-data/20000 User Agents.csv"
    Copy-Item $EvidenceFile "device-detection-dotnet-examples/device-detection-data/20000 Evidence Records.yml"

    Write-Output "Entering '$ExamplesProject'"
    $ExamplesProject = [IO.Path]::Combine($ExamplesRepoPath, "Examples", "ExampleBase")
    Push-Location $ExamplesProject
    
    # Change the dependency version to the locally build Nuget package
    Write-Output "Setting the version of the DeviceDetection package to '$Version'"
    dotnet add package "FiftyOne.DeviceDetection" --version $Version --source "$NugetPackageFolder"
     
    Write-Output "Leaving '$ExamplesProject'"
    Pop-Location


    Write-Output "Building project with following configuration '$Configuration|$Arch|$BuildMethod'"
    .\device-detection-dotnet-examples\ci\build-project.ps1 -RepoName $ExamplesRepoName -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod

    Write-Output "Testing Examples Project"
    .\device-detection-dotnet-examples\ci\run-unit-tests.ps1 -RepoName $ExamplesRepoName -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod
    
}

finally {

    # Write-Output "Leaving '$ExamplesRepoName'"
    # Pop-Location

}

exit $LASTEXITCODE