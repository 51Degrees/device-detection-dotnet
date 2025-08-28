param (
    [Parameter(Mandatory)][string]$RepoName,
    [string]$DeviceDetection,
    [string]$DeviceDetectionUrl
)
$ErrorActionPreference = "Stop"

$deviceDetectionData = "$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data"

./steps/fetch-assets.ps1 -DeviceDetection $DeviceDetection -DeviceDetectionUrl $DeviceDetectionUrl `
    -Assets "TAC-HashV41.hash", "51Degrees-LiteV4.1.hash", "20000 Evidence Records.yml", "20000 User Agents.csv"

Copy-Item "assets/TAC-HashV41.hash" $deviceDetectionData
Copy-Item "assets/51Degrees-LiteV4.1.hash" $deviceDetectionData
Copy-Item "assets/20000 Evidence Records.yml" $deviceDetectionData
Copy-Item "assets/20000 User Agents.csv" $deviceDetectionData
