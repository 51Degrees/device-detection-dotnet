param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName = "device-detection-dotnet",
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$BuildMethod = "msbuild"
)

# DEBUG: Output build environment information
Write-Host "========== DEBUG: Build Environment Information =========="
Write-Host "BuildMethod: $BuildMethod"
Write-Host "Configuration: $Configuration"
Write-Host "Arch: $Arch"
Write-Host "ProjectDir: $ProjectDir"
Write-Host ""
Write-Host "=== .NET SDK Versions ==="
dotnet --list-sdks
Write-Host ""
Write-Host "=== Restoring packages with detailed verbosity for Hash.Tests ==="
dotnet restore "Tests\FiftyOne.DeviceDetection.Hash.Tests\FiftyOne.DeviceDetection.Hash.Tests.csproj" --verbosity normal 2>&1 | Select-String -Pattern "Microsoft.Extensions.Logging"
Write-Host "========== END DEBUG =========="
Write-Host ""

if ($BuildMethod -eq "dotnet"){

    if ($IsWindows) {
        ./dotnet/build-project-core.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch
    }
    else {
        # On non-Windows, use the solution filter to remove the Framework projects.
        ./dotnet/build-project-core.ps1 -RepoName $RepoName -ProjectDir "./FiftyOne.DeviceDetection.Core.slnf" -Name $Name -Configuration $Configuration -Arch $Arch
    }

}
else{
    ./dotnet/build-project-framework.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch
}

exit $LASTEXITCODE