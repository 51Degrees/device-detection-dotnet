param(
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64"
)
$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$perfTests = "$PSScriptRoot/../performance-tests"

$env:PipelineOptions__Elements__0__BuildParameters__DataFile = $env:DEVICEDETECTIONDATAFILE

dotnet build $perfTests -c $Configuration /p:Platform=$Arch
try {
    $server = dotnet "$perfTests/bin/$Arch/$Configuration/net8.0/performance_tests.dll" &
    $results = ./scripts/httpbench.ps1 -HostPort '127.0.0.1:5000' -Endpoint '/process' -CalibrateEndpoint '/calibrate' -UaFile 'assets/20000 User Agents.csv'
} finally {
    Write-Host "--- Performance tests output ---"
    Receive-Job $server
    Stop-Job $server
}

if ($results.overhead_ms -gt 200) {
    Write-Error "Unacceptable request overhead: $($results.overhead_ms)"
} elseif ($results.overhead_ms -lt 0) {
    Write-Error "Request overhead shouldn't be negative: $($results.overhead_ms)"
}

# These results aren't used further, just output them
ConvertTo-Json @{
    'HigherIsBetter' = @{
        'DetectionsPerSecond' = 1/($results.overhead_ms / 1000)
    }
    'LowerIsBetter' = @{
        'MsPerDetection' = $results.overhead_ms
    }
}
