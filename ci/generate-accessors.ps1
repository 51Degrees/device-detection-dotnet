param (
    [Parameter(Mandatory)][string]$RepoName,
    [string]$MetaDataPath = "$PWD/common-metadata",
    [string]$DataType = "HashV41"
)
$ErrorActionPreference = "Stop"

./tools/ci/generate-accessors.ps1 -RepoName:$RepoName -MetaDataPath:$MetaDataPath -DataType:$DataType

Copy-Item "tools/CSharp/IDeviceData.cs" "device-detection-dotnet/FiftyOne.DeviceDetection.Data/Data/"
Copy-Item "tools/CSharp/DeviceDataBase.cs" "device-detection-dotnet/FiftyOne.DeviceDetection.Data/"
