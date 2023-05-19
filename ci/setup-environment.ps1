param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Arch = "x64",
    [string]$Configuration = "Release",
    [hashtable]$Keys
)
$RepoPath = [IO.Path]::Combine($pwd, $RepoName)

if (!$Configuration.Contains("Core")) {

    # Setup the MSBuild environment if it is required.
    ./environments/setup-msbuild.ps1
    ./environments/setup-vstest.ps1
}

if ($IsLinux) {

    # Install multilib, as this may be required.
    sudo apt-get install -y gcc-multilib g++-multilib

}

$env:DEVICEDETECTIONDATAFILE = [IO.Path]::Combine($RepoPath, "FiftyOne.DeviceDetection", "device-detection-cxx", "device-detection-data", "TAC-HashV41.hash")
$env:SUPER_RESOURCE_KEY = $Keys.TestResourceKey