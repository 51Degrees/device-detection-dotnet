
param (
    [string]$RepoName,
    [string]$DeviceDetection,
    [string]$DeviceDetectionUrl
)

function Get-CurrentFileName {
    $MyInvocation.ScriptName
}
function Get-CurrentLineNumber {
    $MyInvocation.ScriptLineNumber
}
$RepoPath = [IO.Path]::Combine($pwd, $RepoName)
$DataFileDir = [IO.Path]::Combine($pwd, $RepoName, "FiftyOne.DeviceDetection", "device-detection-cxx", "device-detection-data")

if ($DeviceDetection -ne "") {
    # Fetch the TAC data file for testing with
    ./steps/fetch-hash-assets.ps1 -RepoName $RepoName -LicenseKey $DeviceDetection -Url $DeviceDetectionUrl
    
    # Move the data file to the correct location
    $DataFileName = "TAC-HashV41.hash"
    $DataFileSource = [IO.Path]::Combine($pwd, $RepoName, $DataFileName)
    $DataFileDestination = [IO.Path]::Combine($DataFileDir, $DataFileName)
    Move-Item $DataFileSource $DataFileDestination
}
else {
    Write-Output "::warning file=$(Get-CurrentFileName),line=$(Get-CurrentLineNumber),endLine=$(Get-CurrentLineNumber),title=No On-Premise Data File::A device detection license was not provided. So Hash data file will not be downloaded."
    Write-Warning "A device detection license was not provided. So Hash data file will not be downloaded."
}

# Get the evidence files for testing. These are in the device-detection-data submodule,
# But are not pulled by default.
Push-Location $DataFileDir
try {
    Write-Output "Pulling evidence files"
    git lfs pull
}
finally {
    Pop-Location
}
