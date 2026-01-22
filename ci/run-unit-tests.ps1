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
Write-Host ""
Write-Host "=== .NET SDK Versions ==="
dotnet --list-sdks
Write-Host ""
Write-Host "=== MSBuild Version ==="
try {
    $msbuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe 2>$null
    if ($msbuildPath) {
        & $msbuildPath -version
    }
} catch {
    Write-Host "MSBuild version check failed"
}
Write-Host ""
Write-Host "=== NuGet Package Versions (Microsoft.Extensions.Logging*) ==="
Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages\microsoft.extensions.logging*" -Directory -ErrorAction SilentlyContinue | ForEach-Object {
    $packageName = $_.Name
    Get-ChildItem -Path $_.FullName -Directory | ForEach-Object {
        Write-Host "$packageName : $($_.Name)"
    }
}
Write-Host ""
Write-Host "=== Assembly Versions in Test Output Directories ==="
$testDirs = Get-ChildItem -Path "Tests" -Directory -Recurse -Filter "bin" -ErrorAction SilentlyContinue
foreach ($dir in $testDirs) {
    $loggingDlls = Get-ChildItem -Path $dir.FullName -Recurse -Filter "Microsoft.Extensions.Logging.Abstractions.dll" -ErrorAction SilentlyContinue
    foreach ($dll in $loggingDlls) {
        $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dll.FullName)
        Write-Host "$($dll.FullName)"
        Write-Host "  FileVersion: $($version.FileVersion)"
        Write-Host "  ProductVersion: $($version.ProductVersion)"
    }
}
Write-Host ""
Write-Host "=== Generated Binding Redirects (*.dll.config files) ==="
$configFiles = Get-ChildItem -Path "Tests" -Directory -Recurse -Filter "*.dll.config" -ErrorAction SilentlyContinue
foreach ($config in $configFiles) {
    if ($config.Name -like "*Tests*.dll.config") {
        Write-Host "File: $($config.FullName)"
        $content = Get-Content $config.FullName -Raw
        if ($content -match "Microsoft.Extensions.Logging") {
            $matches = [regex]::Matches($content, '<dependentAssembly>[\s\S]*?Microsoft\.Extensions\.Logging[\s\S]*?</dependentAssembly>')
            foreach ($match in $matches) {
                Write-Host $match.Value
            }
        }
        Write-Host ""
    }
}
Write-Host "========== END DEBUG =========="
Write-Host ""

./dotnet/run-unit-tests.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod -Filter ".*Tests(|\.Core|\.Web)\.dll"

exit $LASTEXITCODE
