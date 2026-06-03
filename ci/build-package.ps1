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

# Write the strong-name key so CI (PublicSign=false) can full-sign the assemblies.
# The test/PR build path does this in setup-environment.ps1; the publish path runs
# this script instead, so it must write the key too (see CS7027 on Nightly Publish).
[IO.File]::WriteAllBytes("$PSScriptRoot/../51Degrees.snk", [Convert]::FromBase64String($Keys.StrongNameKeyBase64))

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
./dotnet/build-package-nuget.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Version $Version -SolutionName "FiftyOne.DeviceDetection.sln" `
    -CodeSigningKeyVaultUrl $Keys.CodeSigningKeyVaultUrl `
    -CodeSigningKeyVaultClientId $Keys.CodeSigningKeyVaultClientId `
    -CodeSigningKeyVaultTenantId $Keys.CodeSigningKeyVaultTenantId `
    -CodeSigningKeyVaultClientSecret $Keys.CodeSigningKeyVaultClientSecret `
    -CodeSigningKeyVaultCertificateName $Keys.CodeSigningKeyVaultCertificateName

exit $LASTEXITCODE
