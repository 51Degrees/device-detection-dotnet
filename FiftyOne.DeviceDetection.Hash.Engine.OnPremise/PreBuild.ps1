
Param (
	[string]$BuildType = 'Release',
	[string]$Arch = 'x64'
)

# Work out the OS folder
if ($IsWindows) {
	$OS = "windows"
}
if ($IsLinux) {
	$OS = "linux"
}
if ($IsMacOS) {
	$OS = "macos"
}

Write-Output "-- PreBuild '$BuildType' '$Arch' '$OS'"

# https://www.kitware.com//cmake-building-with-all-your-cores/
$Cores = [System.Environment]::ProcessorCount + 1
$Jargs = "-j$Cores"

# Ensure the build path is present
$BuildPath = [IO.Path]::Combine($pwd, "build", $OS, $Arch)
if ($(Test-Path -Path $BuildPath) -eq $False) {

	New-Item -ItemType Directory -Force -Path $BuildPath | Out-Null

}
$TargetPath = [IO.Path]::Combine($BuildPath, $OS, $Arch, $BuildType)
if ($(Test-Path -Path $TargetPath) -eq $False) {

	New-Item -ItemType Directory -Force -Path $TargetPath | Out-Null

}

# Change the current directory to the build path
Push-Location $BuildPath

try {

	if ($IsWindows) {

		if ($Arch -eq "x86") {
			# Map x86 to Win32 is we're building on Windows
			$Arch = "Win32"
		}

		# CMake handles the multi configuration environment including the 
		# optimisation parameters for Release. The output can be checked in 
		# the fiftyone-hash-dotnet.vxproj file to ensure it includes /O2 and
		# other optimisations in Release configuration.

		cmake ../../.. -A $Arch -DRebuildSwig=Off
		cmake --build . -t fiftyone-hash-dotnet --config $BuildType $Jargs
		Copy-Item `
			-Path $BuildPath/$BuildType/FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll `
			-Destination $TargetPath/FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll
		
	}
	else {

		# CMake creates the CMakeFiles\fiftyone-hash-dotnet.dir folder for the
		# target. These files should be checked to ensure they include -O3 and
		# other optimisations in Release configuration.

		$Is32 = "off"
		if ($Arch -eq "x86") {
			$Is32 = "on"
		}
		cmake ../../.. "-D32bit=$Is32" "-DCMAKE_BUILD_TYPE=$BuildType" -DRebuildSwig=Off
		cmake --build . -t fiftyone-hash-dotnet $Jargs
		Copy-Item `
			-Path $BuildPath/$BuildType/FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.so `
			-Destination $TargetPath/FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.so

	}

}
finally {
	
	# Return the current directory to the original no matter what happens 
	Pop-Location

}