param(
    [string]$ProjectDir = ".",
    [string]$RepoName,
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [Parameter(Mandatory=$true)]
    [string]$Version,
    [Parameter(Mandatory=$true)]
    [Hashtable]$Keys
)

# Path to this repository
$BinaryFilesFolder = [IO.Path]::Combine($pwd, $RepoName)

# Path to where the dll files are downloaded for all the platfoms
$PackageFilesPath = [IO.Path]::Combine($pwd, "package-files")

# Copy files over from target to package-files folder
$Files = Get-ChildItem -Path $PackageFilesPath/* -Recurse -Include "linux", "windows", "macos"
foreach($file in $Files){
    Write-Output "Copying '$file' into '$BinaryFilesFolder'"
    Copy-Item -Path $file -Destination $BinaryFilesFolder -Recurse -Force
}

ls $BinaryFilesFolder
./dotnet/build-package-nuget.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Version $Version -SolutionName "FiftyOne.DeviceDetection.sln" -CodeSigningCert $Keys['CodeSigningCert'] -CodeSigningCertPassword $Keys['CodeSigningCertPassword'] 

exit $LASTEXITCODE
