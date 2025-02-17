
./tools/ci/generate-dd-accessors.ps1

$ToolsPath = [IO.Path]::Combine($pwd, "tools")
$DdPath = [IO.Path]::Combine($pwd, "device-detection-dotnet")

Copy-Item "$ToolsPath/CSharp/IDeviceData.cs" "$DdPath/FiftyOne.DeviceDetection.Data/Data/"
Copy-Item "$ToolsPath/CSharp/DeviceDataBase.cs" "$DdPath/FiftyOne.DeviceDetection.Data/"