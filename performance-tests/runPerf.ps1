param ($c, $d, $p)

if ($c -eq $null) {
    Write-Host "Config was not provided, defaulting to Debug."
	$c="Debug"
}
if ($d -eq $null) {
	Write-Host "Dotnet path was not provided, defaulting to system dotnet."
	$d="dotnet"
}
if ($p -eq $null) {
	Write-Host "Platform was not provided, defaulting to x64."
	$p="x64"
}

if ($c -eq $null -or $c -eq $null) {
	Write-Host "Available options are"
    Write-Host "    -c : configuration e.g. Debug or Release"
	Write-Host "    -p : platform e.g. x86, x64 or AnyCPU"
    Write-Host "    -d : path to dotnet executable"
}

Write-Host "Configuration         = $c"
Write-Host "Platform              = $p"
Write-Host "DotNet                = $d"

$scriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

# Constants
$PASSES=20000
$DLLPATH="$scriptRoot/../"
$DLL="bin/$p/$c/netcoreapp3.1/performance_tests.dll"
$SERVICEHOST="localhost:5000"
$CAL="calibrate"
$PRO="process"
$PERF="$scriptRoot/ApacheBench-prefix/src/ApacheBench-build/bin/runPerf.ps1"

Invoke-Expression "$PERF -n $PASSES -s 'Set-Location $DLLPATH; $d $DLL' -c $CAL -p $PRO -h $SERVICEHOST"
