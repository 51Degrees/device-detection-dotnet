param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$BuildMethod = "dotnet"
)

# DEBUG: Output environment and package version information
Write-Host "========== DEBUG: Environment Information =========="
Write-Host "BuildMethod: $BuildMethod"
Write-Host "Configuration: $Configuration"
Write-Host "Arch: $Arch"
Write-Host "Working Directory: $(Get-Location)"
Write-Host ""

Write-Host "=== .NET SDK Versions ==="
dotnet --list-sdks
Write-Host ""

Write-Host "=== NuGet Package Versions (Microsoft.Extensions.Logging*) ==="
Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages\microsoft.extensions.logging*" -Directory -ErrorAction SilentlyContinue | ForEach-Object {
    $packageName = $_.Name
    Get-ChildItem -Path $_.FullName -Directory | ForEach-Object {
        Write-Host "$packageName : $($_.Name)"
    }
}
Write-Host ""

Write-Host "=== net462 Test Output Directory Contents ==="
$net462Path = "Tests\FiftyOne.DeviceDetection.Hash.Tests\bin\$Arch\$Configuration\net462"
Write-Host "Checking path: $net462Path"
if (Test-Path $net462Path) {
    Write-Host "Directory exists. Files:"
    Get-ChildItem -Path $net462Path -Filter "*.dll" | ForEach-Object {
        Write-Host "  $($_.Name)"
    }
    Write-Host ""
    Write-Host "Config files:"
    Get-ChildItem -Path $net462Path -Filter "*.config" | ForEach-Object {
        Write-Host "  $($_.Name)"
    }
} else {
    Write-Host "Directory does NOT exist!"
}
Write-Host ""

Write-Host "=== Microsoft.Extensions.Logging.Abstractions.dll Version ==="
$dllPath = "$net462Path\Microsoft.Extensions.Logging.Abstractions.dll"
if (Test-Path $dllPath) {
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dllPath)
    Write-Host "FileVersion: $($version.FileVersion)"
    Write-Host "ProductVersion: $($version.ProductVersion)"
    # Get assembly version
    $assembly = [System.Reflection.Assembly]::LoadFile((Resolve-Path $dllPath))
    Write-Host "AssemblyVersion: $($assembly.GetName().Version)"
} else {
    Write-Host "DLL not found at: $dllPath"
}
Write-Host ""

Write-Host "=== FiftyOne.DeviceDetection.Hash.Tests.dll.config Contents ==="
$configPath = "$net462Path\FiftyOne.DeviceDetection.Hash.Tests.dll.config"
if (Test-Path $configPath) {
    Write-Host "Config file contents:"
    Get-Content $configPath
} else {
    Write-Host "Config file not found at: $configPath"
}
Write-Host ""
Write-Host "========== END DEBUG =========="
Write-Host ""

./dotnet/run-unit-tests.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod -Filter ".*Tests(|\.Core|\.Web)\.dll"

exit $LASTEXITCODE
