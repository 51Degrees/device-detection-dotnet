param(
    [Parameter(Mandatory)][string]$RepoName,
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Arch = "x64",
    [string]$Configuration = "Release",
    [string]$BuildMethod,
    [string]$StrongNameKeyBase64,
    [hashtable]$Keys
)
$RepoPath = [IO.Path]::Combine($pwd, $RepoName)

if ($BuildMethod -ne "dotnet") {

    # Setup the MSBuild environment if it is required.
    ./environments/setup-msbuild.ps1
    ./environments/setup-vstest.ps1
}

if ($IsLinux) {
    sudo apt-get update
    # Install multilib, as this may be required. It exists only on x86, where
    # it supplies the 32 bit libraries a 32 bit build links against. There is
    # no arm64 package under either name, so on an ARM runner apt answers
    # "has no installation candidate" and "Unable to locate package". That was
    # harmless while a failing script was ignored, and it now stops the step.
    if ([Runtime.InteropServices.RuntimeInformation]::OSArchitecture -eq 'X64') {
        sudo apt-get install -y gcc-multilib g++-multilib
    }
}

$env:_51DEGREES_DD_PATH = "$PWD/$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data/TAC-HashV41.hash"
$env:_51DEGREES_RESOURCE_KEY = $Keys.TestResourceKey
$env:ACCEPTCH_BROWSER_KEY = $Keys.AcceptCHBrowserKey
$env:ACCEPTCH_HARDWARE_KEY = $Keys.AcceptCHHardwareKey
$env:ACCEPTCH_PLATFORM_KEY = $Keys.AcceptCHPlatformKey
$env:ACCEPTCH_NONE_KEY = $Keys.AcceptCHNoneKey

# Legacy environment variables, checked if the new ones aren't set
$env:SUPER_RESOURCE_KEY = $env:_51DEGREES_RESOURCE_KEY

[IO.File]::WriteAllBytes("$PSScriptRoot/../51Degrees.snk", [Convert]::FromBase64String($StrongNameKeyBase64))
