param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName,
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$BuildMethod = "dotnet"
)

if ($BuildMethod -eq "dotnet") {

    ./dotnet/run-unit-tests.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch -BuildMethod $BuildMethod -Filter ".*Tests(|\.Core|\.Web)\.dll"

}
else {

    # When using vstest.console.exe (non-dotnet BuildMethod), we must pass
    # --InIsolation to run .NET Framework (net462) test assemblies in a
    # separate testhost process. Without this, vstest.console.exe runs
    # net462 tests in its own .NET Core process which cannot apply binding
    # redirects from .dll.config files, causing FileNotFoundException for
    # transitive dependencies like Microsoft.Extensions.Logging.Abstractions.

    $RepoPath = [IO.Path]::Combine($pwd, $ProjectDir)
    $Filter = ".*Tests(|\.Core|\.Web)\.dll"
    $TestResultPath = [IO.Path]::Combine($pwd, "test-results", "unit", $Name)

    New-Item -ItemType Directory -Path $TestResultPath -Force | Out-Null

    $ok = $true

    foreach ($NextFile in (Get-ChildItem -Path $RepoPath -Recurse -File)) {
        if ($NextFile.DirectoryName -notlike "*\bin\*") { continue }
        if ($NextFile.Name -like "*performance*") { continue }
        if ($NextFile.Name -notmatch $Filter) { continue }

        Write-Output "Testing Assembly: '$($NextFile.FullName)'"

        $PlatformParams = @()
        if ($Arch -ne "Any CPU" -and $NextFile.Name -notlike "*.dll") {
            $PlatformParams = @("/Platform:$Arch")
        }

        & vstest.console.exe $NextFile.FullName `
            @PlatformParams `
            /Logger:trx `
            /ResultsDirectory:$TestResultPath `
            --InIsolation

        Write-Output "vstest.console LastExitCode=$LASTEXITCODE"
        if ($LASTEXITCODE -ne 0) { $ok = $false }
    }

    if (-not $ok) {
        Write-Error "One or more test runs failed."
        exit 1
    }

}

exit $LASTEXITCODE