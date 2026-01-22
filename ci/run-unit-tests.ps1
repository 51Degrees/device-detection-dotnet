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
Write-Host "ProjectDir: $ProjectDir"
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

# The actual project is in the ProjectDir subdirectory (or current dir if ProjectDir is ".")
$basePath = if ($ProjectDir -eq ".") { "." } else { $ProjectDir }
$net462Path = Join-Path $basePath "Tests\FiftyOne.DeviceDetection.Hash.Tests\bin\$Arch\$Configuration\net462"

Write-Host "=== net462 Test Output Directory Contents ==="
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
    Write-Host "Directory does NOT exist at: $net462Path"
    # Try to find it
    Write-Host "Searching for net462 directories..."
    Get-ChildItem -Path $basePath -Recurse -Directory -Filter "net462" -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host "  Found: $($_.FullName)"
    }
}
Write-Host ""

Write-Host "=== Microsoft.Extensions.Logging.Abstractions.dll Version ==="
$dllPath = Join-Path $net462Path "Microsoft.Extensions.Logging.Abstractions.dll"
if (Test-Path $dllPath) {
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dllPath)
    Write-Host "FileVersion: $($version.FileVersion)"
    Write-Host "ProductVersion: $($version.ProductVersion)"
    # Get assembly version
    $assembly = [System.Reflection.Assembly]::LoadFile((Resolve-Path $dllPath))
    Write-Host "AssemblyVersion: $($assembly.GetName().Version)"
} else {
    Write-Host "DLL not found at: $dllPath"
    # Try to find it
    Write-Host "Searching for Microsoft.Extensions.Logging.Abstractions.dll..."
    Get-ChildItem -Path $basePath -Recurse -Filter "Microsoft.Extensions.Logging.Abstractions.dll" -ErrorAction SilentlyContinue | Select-Object -First 5 | ForEach-Object {
        $v = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($_.FullName)
        Write-Host "  Found: $($_.FullName) - FileVersion: $($v.FileVersion)"
    }
}
Write-Host ""

Write-Host "=== FiftyOne.DeviceDetection.Hash.Tests.dll.config Contents ==="
$configPath = Join-Path $net462Path "FiftyOne.DeviceDetection.Hash.Tests.dll.config"
if (Test-Path $configPath) {
    Write-Host "Config file contents:"
    Get-Content $configPath
} else {
    Write-Host "Config file not found at: $configPath"
    # Try to find it
    Write-Host "Searching for *.dll.config files..."
    Get-ChildItem -Path $basePath -Recurse -Filter "*.dll.config" -ErrorAction SilentlyContinue | Where-Object { $_.Name -like "*Tests*" } | Select-Object -First 5 | ForEach-Object {
        Write-Host "  Found: $($_.FullName)"
        Write-Host "  Contents:"
        Get-Content $_.FullName | ForEach-Object { Write-Host "    $_" }
        Write-Host ""
    }
}
Write-Host ""
Write-Host "========== END DEBUG =========="
Write-Host ""

./dotnet/run-unit-tests.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod -Filter ".*Tests(|\.Core|\.Web)\.dll"

exit $LASTEXITCODE
