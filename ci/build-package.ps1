param(
    [string]$ProjectDir = ".",
    [string]$RepoName,
    [string]$Name = "Release_x64",
    [string]$Configuration = "CoreRelease",
    [string]$Arch = "x64",
    [Parameter(Mandatory=$true)]
    [string]$Version
)

# Path to where the dll files are downloaded for all the platfoms
$PackageFilesPath = [IO.Path]::Combine($pwd, "package-files")

# Copy files over from target to package-files folder
$Files = Get-ChildItem -Path $PackageFilesPath/* -Recurse -Include "linux", "windows" 
foreach($file in $Files){
    Copy-Item -Path $file -Destination $PackageFilesPath -Recurse -Force
}

./dotnet/build-package-nuget.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -Version $Version -SolutionName "FiftyOne.DeviceDetection"

exit $LASTEXITCODE
