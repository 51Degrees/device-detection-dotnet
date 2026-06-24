# Contributing

If you wish to contribute to this project, please see the shared 51Degrees
[guidance](https://github.com/51Degrees/common-ci/blob/main/CONTRIBUTING.md).

## Considerations

When new projects are added, be sure to update the `.slnf` file too.
This is a solution filter which loads only dotnet core projects, and
can be used to build on platforms like Linux which do not have support
for Framework.

## Building locally

`FiftyOne.DeviceDetection.Hash.Engine.OnPremise` has a native C++ core that is
compiled by CMake during the build (`PreBuild.ps1`). CMake must be able to target
the installed Visual Studio toolchain. If a system-wide CMake that predates your
Visual Studio version is first on `PATH`, the configure step fails with
`CMAKE_CXX_COMPILER not set` or a generator mismatch
(`Does not match the generator used previously`).

Use the CMake bundled with Visual Studio (it always matches the installed VS
generator) by putting it first on `PATH`, e.g. for VS 2022/18 on Windows:

```powershell
$env:PATH = "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin;$env:PATH"
dotnet build -p:Platform=x64
```

If you hit the generator-mismatch error after switching CMake versions, delete the
stale native build dir and rebuild:
`FiftyOne.DeviceDetection.Hash.Engine.OnPremise/build/`.

### Test projects

Shared test configuration (the `Microsoft.NET.Test.Sdk`, `Moq` and `MSTest`
package references) lives in `Tests/Directory.Build.props` and is applied to any
project under `Tests/` that sets `<IsTestProject>true</IsTestProject>`. The
non-test projects in that folder (`TestHelpers`, `Tests.Framework`,
`PackageConsumption`) deliberately do not set it. MSTest's analyzers are enabled
via the `MSTest` meta-package; their rules are build-breaking under
`TreatWarningsAsErrors`, so new test code must follow the MSTest analyzer patterns.