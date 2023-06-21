
Param (
	[string]$BuildType = 'Release',
	[string]$Arch = 'x64'
)

Push-Location ..

try {

	if ($(Test-Path -Path build) -eq $False) {
	
		mkdir build
	
	}

	Push-Location build
	
	try {

		Remove-Item -Force -Recurse *
		if ($($env:Os).Contains("Windows")) {
			if ($BuildType.Contains("Release")) {
				$BuildType = "Release"
			}
			elseif ($BuildType.Contains("Debug")) {
				$BuildType = "Debug"
			}
			else {
				Write-Error "Build type '$BuildType' not recognized. Must contain Release or Debug."
				exit 1
			}

			if ($Arch -eq "x86") {
				# Map x86 to Win32 is we're building on Windows
				$Arch = "Win32"
			}
			else {
				$Arch = "x64"
			}
			cmake .. -DCMAKE_BUILD_TYPE=$BuildType -A $Arch
			cmake --build . -t fiftyone-hash-dotnet --config $BuildType	
		}
		else {
			$Is32 = "off"
			if ($Arch -eq "x86") {
				$Is32 = "on"
			}
			cmake .. -DCMAKE_BUILD_TYPE=$BuildType -D32bit=$Is32
			cmake --build . -t fiftyone-hash-dotnet
		}

		
	}
	finally {
		
		Pop-Location

	}

}
finally {

	Pop-Location

}