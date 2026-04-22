param(
    [Parameter(Mandatory)][string]$RepoName,
    [Parameter(Mandatory)][string]$OrgName,
    [string]$BuildMethod = "dotnet",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$ExamplesRepo = "device-detection-dotnet-examples",
    [string]$ExamplesBranch = "version/4.5",
    [string]$Version
)
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 1.0

# If Version is not provided, the script is running in a workflow that doesn't
# build packages and the integration tests will be skipped
if (!$Version) {
    Write-Host "Skipping integration tests"
    exit 0
}

Write-Host "Fetching examples..."
./steps/clone-repo.ps1 -RepoName $ExamplesRepo -OrgName $OrgName -Branch $ExamplesBranch
# Shorter name for Windows compatibility. -PassThru parameter doesn't work,
# 'Name' property is empty for some reason
Rename-Item $ExamplesRepo "ex"
$ExamplesRepo = "ex"

Write-Host "Moving TAC and evidence files for examples..."
foreach ($_ in "20000 User Agents.csv", "20000 Evidence Records.yml", "51Degrees-LiteV4.1.hash", "TAC-HashV41.hash") {
    Write-Host "Linking $_"
    New-Item -ItemType SymbolicLink -Force -Target "$PWD/$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data/$_" -Path "$ExamplesRepo/device-detection-data/$_"
}

# Install the nuget packages to the local feed. The packages in the 'package'
# folder must be pushed to local feed and cannot be used directly, as all the
# other dependencies will be installed in the local feed.
$localFeed = New-Item -ItemType Directory -Force "$HOME/.nuget/packages"
dotnet nuget add source $localFeed
dotnet nuget push -s $localFeed (Get-ChildItem -Path package -Filter '*.nupkg')

Write-Host "Restoring Examples Project..."
dotnet restore $ExamplesRepo
Get-ChildItem -Path $ExamplesRepo -Recurse -File -Filter '*.csproj' | ForEach-Object {
    Write-Host "Setting DeviceDetection version for '$_' to $Version"
    dotnet add $_ package "FiftyOne.DeviceDetection" --version $Version
}

Write-Host "Building Examples Project..."
& $ExamplesRepo/ci/build-project.ps1 -RepoName $ExamplesRepo -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod
Write-Host "Testing Examples Project..."
./dotnet/run-integration-tests.ps1 -RepoName $ExamplesRepo -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod -DirNameFormatForDotnet "*" -DirNameFormatForNotDotnet "*" -Filter ".*\.sln"

Copy-Item $ExamplesRepo/test-results $RepoName -Recurse

# Package consumption validation. Guards against regressions in the package
# .targets file where the native DLL ends up missing or wrong-platform in
# publish output. Publishes a small fixture (Tests/PackageConsumption)
# against the just-packed .nupkg and executes the resulting binary; if the
# native DLL is missing or incompatible, the process crashes at engine
# construction and the step fails via $PSNativeCommandUseErrorActionPreference.
Write-Host "Running package consumption validation..."

$Fixture  = "$PWD/$RepoName/Tests/PackageConsumption"
$DataFile = (Resolve-Path "$PWD/$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data/51Degrees-LiteV4.1.hash").Path

# --use-current-runtime triggers the same flatten behaviour as an explicit
# --runtime <rid> (collapses runtimes/<rid>/native/ into the output root),
# which is the layout in which .targets-file regressions actually manifest
# at load time. Portable publish preserves the runtimes/ subtree and .NET's
# native resolver bypasses a wrongly-copied DLL in the root, hiding bugs.
dotnet publish "$Fixture/PackageConsumption.csproj" `
    --framework net8.0 `
    --use-current-runtime `
    --configuration Release `
    --self-contained false `
    -o "$Fixture/publish-modern" `
    "/p:PackageConsumptionVersion=$Version"
& "$Fixture/publish-modern/PackageConsumption" $DataFile

# net48 leg guards the .NETFramework copy path — Framework doesn't use
# runtimes/<rid>/native resolution, so a missing or wrong DLL in the output
# root surfaces immediately at engine construction.
if ($IsWindows) {
    dotnet publish "$Fixture/PackageConsumption.csproj" `
        --framework net48 `
        --configuration Release `
        --self-contained false `
        -o "$Fixture/publish-net48" `
        "/p:PackageConsumptionVersion=$Version"

    Write-Host "=== net48 publish output ==="
    Get-ChildItem "$Fixture/publish-net48" -Filter "*Native*" -Recurse | ForEach-Object { Write-Host $_.FullName }
    Get-ChildItem "$Fixture/publish-net48" -Filter "*.targets" -Recurse | ForEach-Object { Write-Host $_.FullName }
    $PkgDir = "$HOME/.nuget/packages/fiftyone.devicedetection.hash.engine.onpremise/$Version"
    Write-Host "=== NuGet package contents ==="
    if (Test-Path $PkgDir) {
        Get-ChildItem $PkgDir -Recurse -Filter "*Native*" | ForEach-Object { Write-Host $_.FullName }
        if (Test-Path "$PkgDir/build") { Get-ChildItem "$PkgDir/build" | ForEach-Object { Write-Host $_.FullName } }
    } else {
        Write-Host "Package dir not found: $PkgDir"
    }
    Write-Host "=== end diagnostics ==="

    & "$Fixture/publish-net48/PackageConsumption" $DataFile
}

Write-Host "Package consumption validation passed"
