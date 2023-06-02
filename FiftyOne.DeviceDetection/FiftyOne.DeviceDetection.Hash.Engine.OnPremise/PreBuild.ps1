
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
		if ($($env:Os).Contains("Windows") -and $Arch -eq "x86") {

			# Map x86 to Win32 is we're building on Windows
			$Arch = "Win32"

		}
		cmake .. -DCMAKE_BUILD_TYPE=$BuildType -A $Arch
		cmake --build . -t fiftyone-hash-dotnet

	}
	finally {
		
		Pop-Location

	}

}
finally {

	Pop-Location

}