
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