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

    # Use the local dev library in the examples.
    $exampleBase = "$seleniumExamples/Examples/ExampleBase/FiftyOne.DeviceDetection.Examples.csproj"
    (Get-Content $exampleBase) -replace `
        '<PackageReference Include="FiftyOne.DeviceDetection" Version="[^"]*" />', `
        "<ProjectReference Include=`"../../../$RepoName/FiftyOne.DeviceDetection/FiftyOne.DeviceDetection.csproj`" />" |
        Set-Content $exampleBase

    # Match the examples' Pipeline.Web version to the library.
    $cloudCsproj = "$RepoName/FiftyOne.DeviceDetection.Cloud/FiftyOne.DeviceDetection.Cloud.csproj"
    $pipeMatch = Select-String -Path $cloudCsproj -Pattern 'FiftyOne\.Pipeline\.CloudRequestEngine" Version="([0-9.]+)"'
    $pipever = if ($pipeMatch) { $pipeMatch.Matches[0].Groups[1].Value } else { "4.5.46" }
    $cloudProject = "$seleniumExamples/Examples/Cloud/GettingStarted-Web"
    $onPremProject = "$seleniumExamples/Examples/OnPremise/GettingStarted-Web"
    foreach ($webCsproj in "$cloudProject/GettingStarted-Web.csproj", "$onPremProject/GettingStarted-Web.csproj") {
        (Get-Content $webCsproj) -replace `
            'Include="FiftyOne.Pipeline.Web" Version="[^"]*"', `
            "Include=`"FiftyOne.Pipeline.Web`" Version=`"$pipever`"" |
            Set-Content $webCsproj
    }

    # Get the shared contract tests.
    if (-not (Test-Path selenium-api-tests)) {
        git clone --depth 1 https://github.com/51Degrees/selenium-api-tests.git
    }

    # Starts one web example on the given port, runs the shared Contract
    # tests against it, and stops it again. Variables in $ExampleEnv are set
    # for the example and removed afterwards. A failing test throws, because
    # $PSNativeCommandUseErrorActionPreference is set at the top of the script.
    function Invoke-ContractTests {
        param(
            [Parameter(Mandatory)][string]$Label,
            [Parameter(Mandatory)][string]$Project,
            [Parameter(Mandatory)][int]$Port,
            [string[]]$BuildArgs = @(),
            [hashtable]$ExampleEnv = @{}
        )
        Write-Host "::group::Selenium Contract tests against the $Label example on port $Port"
        $example = $null
        try {
            dotnet build $Project @BuildArgs

            foreach ($name in $ExampleEnv.Keys) {
                Set-Item -Path "Env:$name" -Value $ExampleEnv[$name]
            }
            $env:PORT = $Port
            $env:ASPNETCORE_URLS = "http://localhost:$Port"
            $example = dotnet run --no-build --project $Project @BuildArgs --no-launch-profile 2>&1 &

            # Wait for the example to come up.
            curl -sS -o $(if ($IsWindows) { 'NUL' } else { '/dev/null' }) --retry 5 --retry-connrefused "http://localhost:$Port"

            $env:CLOUD_ROOT_URL = "https://cloud.51degrees.com/"
            $env:PAID_RESOURCE_KEY = $TestResourceKey
            $env:EXAMPLE_URL = "http://localhost:$Port"
            $env:EXAMPLE_LANG = 'dotnet'
            dotnet test selenium-api-tests -c Release --filter TestCategory=Contract
        } catch {
            if ($example) { Write-Host ">>> $Label example output >>>"; Receive-Job $example | Out-Host; Write-Host '<<< app output <<<' }
            throw
        } finally {
            if ($example) { Remove-Job -Force $example }
            Remove-Item Env:ASPNETCORE_URLS, Env:PORT, Env:EXAMPLE_URL -ErrorAction SilentlyContinue
            foreach ($name in $ExampleEnv.Keys) {
                Remove-Item -Path "Env:$name" -ErrorAction SilentlyContinue
            }
            Write-Host "::endgroup::"
        }
    }

    # Run both examples even if the first fails, then fail the step if either
    # did, so one run reports both results.
    $contractFailures = @()

    # The cloud example, pointed at the live cloud.
    try {
        Invoke-ContractTests -Label 'cloud' -Project $cloudProject -Port 8095 `
            -BuildArgs @('-c', 'Release') `
            -ExampleEnv @{ FIFTYONE_CLOUD_ENDPOINT = "https://cloud.51degrees.com/api/v4/" }
    } catch {
        Write-Host "::error title=Selenium contract (cloud)::$_"
        $contractFailures += 'cloud'
    }

    # The on-premise example, using the TAC data file, which is the only data
    # file fetched here that has DeviceType and the JavaScript screen size
    # overrides the Contract tests need. The example is built for the job's
    # platform so that the native engine built by build-project.ps1 is
    # copied next to it, which also means it needs the job's configuration.
    $tacFile = "$PWD/$RepoName/FiftyOne.DeviceDetection.Hash.Engine.OnPremise/device-detection-cxx/device-detection-data/TAC-HashV41.hash"
    if ($Arch -eq 'x86') {
        Write-Host "::warning title=Selenium on-premise skipped::The native engine on this job is built for x86 only, so the example would have to run as an x86 process, which this script does not set up. The on-premise Contract tests run on the x64 and arm64 jobs."
    } elseif (-not (Test-Path $tacFile)) {
        Write-Host "::warning title=Selenium on-premise skipped::No TAC data file at '$tacFile', which needs the device detection licence, so the on-premise Contract tests were not run."
    } else {
        try {
            Invoke-ContractTests -Label 'on-premise' -Project $onPremProject -Port 8096 `
                -BuildArgs @('-c', $Configuration, "--property:Platform=$Arch") `
                -ExampleEnv @{ '51DEGREES_DD_PATH' = (Resolve-Path $tacFile).Path }
        } catch {
            Write-Host "::error title=Selenium contract (on-premise)::$_"
            $contractFailures += 'on-premise'
        }
    }

    Remove-Item Env:CLOUD_ROOT_URL, Env:PAID_RESOURCE_KEY, Env:EXAMPLE_LANG -ErrorAction SilentlyContinue
    if ($contractFailures) {
        throw "Selenium Contract tests failed for the $($contractFailures -join ' and ') example."
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
# Run one test assembly at a time. Passing the solution instead made VSTest
# start a test host per project simultaneously, and the three web example test
# projects then raced for the fixed Kestrel ports declared in the examples repo
# (Examples/ExampleBase/Constants.cs: 5101 HTTP, 5001 and 5002 HTTPS). Whichever
# host bound second failed with EADDRINUSE, which surfaced as a TestCleanup
# exception on every test in that assembly. See issue #866.
#
# FiftyOne.DeviceDetection.Example.Tests.Web holds the shared base classes and
# declares no tests of its own, so it is excluded - dotnet test reports "No test
# is available" and exits non-zero for an assembly it finds nothing in.
#
# The default DirNameFormat values are kept so that only the build output under
# 'bin' is considered. Overriding them to '*', as this call used to, also matched
# the intermediate copy under 'obj' and would run every assembly twice.
#
# Trade-off: naming assemblies rather than the solution bypasses MSBuild, so the
# RunSettingsFilePath that Tests.Cloud and Tests.OnPremise point at
# (Tests/test.runsettings, MSTest TestTimeout 60s) no longer applies to them; the
# runner's --blame-hang-timeout of 5m is the remaining guard. common-ci cannot
# carry it either way today - dotnet/run-unit-tests.ps1 looks for test.runsettings
# with [IO.Path]::Exists, which resolves against the process directory and not the
# repository it has pushed into, and dotnet/run-integration-tests.ps1 does not
# forward -BlameHangTimeout. Both belong in common-ci.
$exampleTestAssemblies = '.*Example\.Tests\.(?!Web\.dll$).*\.dll$'

# A filter that matches nothing leaves the run green with no tests executed,
# which is how the Selenium tests went unnoticed until examples PR #920. Fail
# loudly instead, and record what is about to run.
$exampleTestAssemblyFiles = Get-ChildItem -Path $ExamplesRepo -Recurse -File |
    Where-Object { $_.DirectoryName -like '*bin*' -and $_.Name -match $exampleTestAssemblies }
if (-not $exampleTestAssemblyFiles) {
    throw "No example test assemblies under '$ExamplesRepo' matched '$exampleTestAssemblies'."
}
Write-Host "Example test assemblies to run:"
$exampleTestAssemblyFiles | ForEach-Object { Write-Host "  $($_.FullName)" }

# common-ci's runner is written to keep going after a failing assembly: it records
# the failure, carries on, and calls Write-Error once the loop is done. The
# $PSNativeCommandUseErrorActionPreference set at the top of this script is
# inherited into it though, so the first non-zero dotnet test / vstest.console.exe
# throws and the remaining assemblies never run. That went unnoticed while the
# solution was passed as a single invocation; with one invocation per assembly it
# hides real failures - Windows_x64 stopped after three of the five assemblies in
# run 33748849603. Turn it off for this call only. The step still fails, because
# the runner's closing Write-Error is terminating under $ErrorActionPreference.
$PSNativeCommandUseErrorActionPreference = $false
try {
    ./dotnet/run-integration-tests.ps1 -RepoName $ExamplesRepo -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod -Filter $exampleTestAssemblies
} finally {
    $PSNativeCommandUseErrorActionPreference = $true
}

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
