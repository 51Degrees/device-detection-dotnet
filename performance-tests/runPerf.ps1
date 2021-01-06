param ($c, $d)

if ($c -eq $null) {
    Write-Host "Config was not provided, defaulting to Debug."
	$c="Debug"
}
if ($d -eq $null) {
	Write-Host "Dotnet path was not provided, defaulting to system dotnet."
	$d="dotnet"
}

if ($c -eq $null -or $c -eq $null) {
	Write-Host "Available options are"
    Write-Host "    -c : configuration e.g. Debug or Release"
    Write-Host "    -d : path to dotnet executable"
}

Write-Host "Configuration         = $c"
Write-Host "Platform              = $p"
Write-Host "DotNet                = $d"

$scriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

# Constants
$PASSES=20000
$PROJECT="$scriptRoot/.."
$SERVICEHOST="localhost:5000"
$CAL="calibrate"
$PRO="process"
$PERF="$scriptRoot/ApacheBench-prefix/src/ApacheBench-build/bin/runPerf.ps1"

Invoke-Expression "$PERF -n $PASSES -s '$d run -c $c --no-build --project $PROJECT' -c $CAL -p $PRO -h $SERVICEHOST"
