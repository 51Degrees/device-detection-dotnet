param (
    [Parameter(Mandatory)][string]$RepoName,
    [string]$DataFile = "$RepoName/TAC-HashV41.hash"
)
$ErrorActionPreference = "Stop"

./tools/ci/generate-accessors.ps1 @PSBoundParameters

Copy-Item "tools/CSharp/IDeviceData.cs" "device-detection-dotnet/FiftyOne.DeviceDetection.Data/Data/"
Copy-Item "tools/CSharp/DeviceDataBase.cs" "device-detection-dotnet/FiftyOne.DeviceDetection.Data/"
