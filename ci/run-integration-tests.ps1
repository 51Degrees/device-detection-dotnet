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
# the publish output. Publishes a small fixture (Tests/PackageConsumption)
# against the just-packed .nupkg and executes the resulting binary; if the
# native DLL is missing or incompatible, the process crashes at engine
# construction and the step fails.
Write-Host "Running package consumption validation..."

$Fixture  = "$PWD/$RepoName/Tests/PackageConsumption"
$DataFile = (Resolve-Path "$PWD/$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data/51Degrees-LiteV4.1.hash").Path

# Native RID for the current host + matrix entry. We publish for the native
# RID only — cross-publish output can't be executed on this runner anyway.
if ($IsWindows) {
    if     ($Arch -eq 'x86')   { $HostRid = 'win-x86' }
    elseif ($Arch -eq 'ARM64') { $HostRid = 'win-arm64' }
    else                        { $HostRid = 'win-x64' }
    # net48 leg guards against the SDK-style multi-target regression.
    $Tfms = @('net48', 'net8.0')
} elseif ($IsLinux) {
    $HostRid = if ($Arch -eq 'ARM64') { 'linux-arm64' } else { 'linux-x64' }
    $Tfms = @('net8.0')
} elseif ($IsMacOS) {
    $HostRid = if ($Arch -eq 'ARM64') { 'osx-arm64' } else { 'osx-x64' }
    $Tfms = @('net8.0')
} else {
    throw "Unsupported host OS for package consumption validation"
}

foreach ($tfm in $Tfms) {
    Write-Host "::group::Publish + run fixture ($tfm / $HostRid)"

    # net48 doesn't take --runtime (Framework is host-native); all others do.
    $publishArgs = @(
        "$Fixture/PackageConsumption.csproj",
        '--framework',    $tfm,
        '--configuration', 'Release',
        '--self-contained', 'false',
        "/p:PackageConsumptionVersion=$Version"
    )
    if ($tfm -ne 'net48') {
        $publishArgs += @('--runtime', $HostRid)
        $publishDir = "$Fixture/bin/Release/$tfm/$HostRid/publish"
    } else {
        $publishDir = "$Fixture/bin/Release/$tfm/publish"
    }

    dotnet publish @publishArgs

    $exeName = if ($IsWindows) { 'PackageConsumption.exe' } else { 'PackageConsumption' }
    $exe = Join-Path $publishDir $exeName
    if (-not (Test-Path $exe)) {
        throw "Published executable not found at '$exe'"
    }

    & $exe $DataFile
    if ($LASTEXITCODE -ne 0) {
        throw "Package consumption fixture failed: tfm=$tfm rid=$HostRid exit=$LASTEXITCODE"
    }
    Write-Host "::endgroup::"
}

Write-Host "Package consumption validation passed: $($Tfms.Count) TFM(s) on $HostRid"
