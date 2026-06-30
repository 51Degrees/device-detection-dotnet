param(
    [Parameter(Mandatory)][string]$RepoName,
    [Parameter(Mandatory)][string]$OrgName,
    [Parameter(Mandatory=$true)][string]$TestResourceKey,
    [string]$BuildMethod = "dotnet",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$ExamplesRepo = "device-detection-dotnet-examples",
    [string]$ExamplesBranch = "main",
    [string]$Version
)
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 1.0

$skipSeleniumOnArm = $IsLinux -and $Arch -eq 'arm64'
if ($TestResourceKey -and -not $skipSeleniumOnArm) {
    Write-Host 'Running Selenium tests...'
    $seleniumExamples = "dd-examples-selenium"
    if (-not (Test-Path $seleniumExamples)) {
        ./steps/clone-repo.ps1 -RepoName $ExamplesRepo -OrgName $OrgName -Branch $ExamplesBranch
        Rename-Item $ExamplesRepo $seleniumExamples
    }

    # Use the local dev library in the example.
    $exampleBase = "$seleniumExamples/Examples/ExampleBase/FiftyOne.DeviceDetection.Examples.csproj"
    (Get-Content $exampleBase) -replace `
        '<PackageReference Include="FiftyOne.DeviceDetection" Version="[^"]*" />', `
        "<ProjectReference Include=`"../../../$RepoName/FiftyOne.DeviceDetection/FiftyOne.DeviceDetection.csproj`" />" |
        Set-Content $exampleBase

    # Match the example's Pipeline.Web version to the library.
    $cloudCsproj = "$RepoName/FiftyOne.DeviceDetection.Cloud/FiftyOne.DeviceDetection.Cloud.csproj"
    $pipeMatch = Select-String -Path $cloudCsproj -Pattern 'FiftyOne\.Pipeline\.CloudRequestEngine" Version="([0-9.]+)"'
    $pipever = if ($pipeMatch) { $pipeMatch.Matches[0].Groups[1].Value } else { "4.5.46" }
    $webCsproj = "$seleniumExamples/Examples/Cloud/GettingStarted-Web/GettingStarted-Web.csproj"
    (Get-Content $webCsproj) -replace `
        'Include="FiftyOne.Pipeline.Web" Version="[^"]*"', `
        "Include=`"FiftyOne.Pipeline.Web`" Version=`"$pipever`"" |
        Set-Content $webCsproj

    try {
        # Start the cloud example, pointed at the live cloud.
        Push-Location "$seleniumExamples/Examples/Cloud/GettingStarted-Web"
        try {
            $env:PORT = 8095
            $env:ASPNETCORE_URLS = "http://localhost:$env:PORT"
            $env:FIFTYONE_CLOUD_ENDPOINT = "https://cloud.51degrees.com/api/v4/"
            $example = dotnet run -c Release --no-launch-profile 2>&1 &
        } finally { Pop-Location }

        # Get the shared contract tests.
        if (-not (Test-Path selenium-api-tests)) {
            git clone --depth 1 https://github.com/51Degrees/selenium-api-tests.git
        }
        # Wait for the example to come up.
        curl -sS -o /dev/null --retry 5 --retry-connrefused "http://localhost:$env:PORT"

        $env:CLOUD_ROOT_URL = "https://cloud.51degrees.com/"
        $env:PAID_RESOURCE_KEY = $TestResourceKey
        $env:EXAMPLE_URL = "http://localhost:$env:PORT"
        $env:EXAMPLE_LANG = 'dotnet'
        dotnet test selenium-api-tests -c Release --filter TestCategory=Contract
    } catch {
        if ($example) { Write-Host '>>> example app output >>>'; Receive-Job $example | Out-Host; Write-Host '<<< app output <<<' }
        throw
    } finally {
        if ($example) { Remove-Job -Force $example }
        Remove-Item Env:ASPNETCORE_URLS, Env:PORT -ErrorAction SilentlyContinue
    }
} elseif ($skipSeleniumOnArm) {
    Write-Host "::warning title=Selenium skipped::Selenium contract skipped on linux-arm64 (no selenium-manager aarch64 build); covered on x64 and macOS-arm."
} else {
    Write-Host "::warning title=No Resource Key::No resource key; skipping the Selenium contract."
}

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
    "/p:DeviceDetectionVersion=$Version"
dotnet "$Fixture/publish-modern/PackageConsumption.dll" $DataFile

# net48 leg guards the .NETFramework copy path — Framework doesn't use
# runtimes/<rid>/native resolution, so a missing or wrong DLL in the output
# root surfaces immediately at engine construction.
if ($IsWindows) {
    dotnet publish "$Fixture/PackageConsumption.csproj" `
        --framework net48 `
        --configuration Release `
        --self-contained false `
        -o "$Fixture/publish-net48" `
        "/p:DeviceDetectionVersion=$Version"

    Write-Host "=== ALL files in net48 publish output ==="
    Get-ChildItem "$Fixture/publish-net48" -Recurse | ForEach-Object { Write-Host $_.FullName }
    $PkgDir = "$HOME/.nuget/packages/fiftyone.devicedetection.hash.engine.onpremise/$Version"
    Write-Host "=== .targets file content ==="
    $TargetsFile = "$PkgDir/build/FiftyOne.DeviceDetection.Hash.Engine.OnPremise.targets"
    if (Test-Path $TargetsFile) { Get-Content $TargetsFile | Write-Host } else { Write-Host "targets file not found" }
    Write-Host "=== MSBuild evaluation ==="
    dotnet msbuild "$Fixture/PackageConsumption.csproj" `
        --getProperty:_FiftyOneNativeAssetPath `
        --getProperty:_FiftyOneNativeRuntime `
        --getProperty:TargetFrameworkIdentifier `
        --getProperty:UsingMicrosoftNETSdk `
        --getProperty:Platform `
        --getProperty:RuntimeIdentifier `
        --getProperty:PublishDir `
        /p:TargetFramework=net48 `
        "/p:DeviceDetectionVersion=$Version"
    Write-Host "=== end diagnostics ==="

    & "$Fixture/publish-net48/PackageConsumption.exe" $DataFile
}

Write-Host "Package consumption validation passed"
