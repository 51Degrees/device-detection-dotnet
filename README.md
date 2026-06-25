# 51Degrees Device Detection Engines

![51Degrees](https://raw.githubusercontent.com/51Degrees/common-ci/main/images/logo/360x67.png "Data rewards the curious")
**Pipeline API**

[Developer
Documentation](https://51degrees.com/device-detection-dotnet/index.html?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=51degrees-device-detection-engines)

## Introduction

This repository contains the device detection engines for the .NET
implementation of the Pipeline API.

The
[specification](https://github.com/51Degrees/specifications/blob/main/device-detection-specification/README.md)
is also available on GitHub and is recommended reading if you wish to understand
the concepts and design of this API.

## Dependencies

Visual Studio 2022 or later is recommended. Although Visual Studio Code can be
used for working with most of the projects.

The core device detection projects are written in C and C++. The Pipeline
engines are written in C\# and target .NET Standard 2.0.3. Example and test
projects mostly target .NET 6.0 though in some cases, projects are available
targeting other frameworks.

For runtime dependencies, see our
[dependencies](https://51degrees.com/documentation/_info__dependencies.html?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=dependencies)
page. The `ci/options.json` file lists the tested and packaged .NET versions
and operating systems automatic tests are performed with. The solution will
likely operate with other versions.

### Data

The API can either use our cloud service to get its data or it can use a local
(on-premise) copy of the data.

#### Cloud

You will require a [resource
key](https://51degrees.com/documentation/_info__resource_keys.html?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=cloud) to use the
Cloud API. You can create resource keys using our
[configurator](https://configure.51degrees.com/?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=cloud), see our
[documentation](https://51degrees.com/documentation/_concepts__configurator.html?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=cloud)
on how to use this.

The cloud property tiers changed in May 2026, so the examples and
documentation now reflect what is free and what needs a paid subscription. A
free resource key created [here](https://configure.51degrees.com/Wkqxf3Bs?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=51degrees-device-detection-engines) selects
the free tier properties, whilst a key created
[here](https://configure.51degrees.com/hYzn3TV3?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=51degrees-device-detection-engines) also includes the paid properties
used by the examples. See [pricing](https://51degrees.com/pricing?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=51degrees-device-detection-engines) to get a paid
subscription with more properties.

#### On-Premise

In order to perform device detection on-premise, you will need to use a
51Degrees data file. This repository includes a free, 'lite' file in the
'device-detection-data' sub-module that has a significantly reduced set of
properties. To obtain a file with a more complete set of device properties see
the [51Degrees website](https://51degrees.com/pricing?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=on-premise). If you want to use the
lite file, you will need to install [GitLFS](https://git-lfs.github.com/).

On Linux:

```
sudo apt-get install git-lfs
git lfs install
```

Then, navigate to 'device-detection-cxx/device-detection-data' and execute:

```
git lfs pull
```

The examples in the
[device-detection-dotnet-examples](https://github.com/51Degrees/device-detection-dotnet-examples)
repository resolve the data file in the following order:

1.  The `_51DEGREES_DD_PATH` environment variable, set to an explicit path to
    the data file.
2.  A search of the folder hierarchy for the expected data file name.
3.  The free 'Lite' data file in the device-detection-data submodule.

## Solutions and projects

-   **FiftyOne.DeviceDetection** - Device detection engines and related
    projects.
    -   **FiftyOne.DeviceDetection** - Contains device detection engine
        builders.
    -   **FiftyOne.DeviceDetection.Cloud** - A .NET engine which retrieves
        device detection results by consuming the 51Degrees cloud service. This
        can be swapped out with either the hash or pattern engines seamlessly.
    -   **FiftyOne.DeviceDetection.Hash.Engine.OnPremise** - .NET implementation
        of the device detection hash engine. CMake is used to build the native
        binaries.
    -   **FiftyOne.DeviceDetection.Shared** - Shared classes used by the device
        detection engines.

## Installation

### Nuget

The easiest way to install is to use NuGet to add the reference to the package:

```
Install-Package FiftyOne.DeviceDetection
```

### Build from Source

Device detection on-premise uses a native binary (i.e. compiled from C code to
target a specific platform/architecture). The NuGet package contains several
binaries for common platforms. However, in some cases, you'll need to build the
native binaries yourself for your target platform. This section explains how to
do this.

#### Pre-requisites

-   Install C build tools:
    -   Windows:
        -   You will need either Visual Studio 2022 or the [C++ Build
            Tools](https://visualstudio.microsoft.com/visual-cpp-build-tools/)
            installed.
            -   Minimum platform toolset version is `v143`
            -   Minimum Windows SDK version is `10.0.18362.0`
    -   Linux/MacOS:
        -   `sudo apt-get install g++ make libatomic1`
-   If you have not already done so, pull the git submodules that contain the
    native code:
    -   `git submodule update --init --recursive`

Visual studio should now be able to build the native binaries as part of its
normal build process.

#### Packaging

You can package a project into NuGet `*.nupkg` file by running a command like:

```text
dotnet pack [Project] -o "[PackagesFolder]" /p:PackageVersion=0.0.0 -c [Configuration] /p:Platform=[Architecture]
```

##### ⚠️ Notes on packaging `FiftyOne.DeviceDetection.Hash.Engine.OnPremise`

📝 Using `AnyCPU` might prevent the unmanaged (C++) code from being built into `.Native.dll` library. Use `x86`/`x64`/`arm64` specifically.

📝 If creating cross-platform package from multiple native dlls, put all 6x `FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll` into respective folders:

```text
../
    macos/
        arm64/
        x64/
    linux/
        x64/
        x86/
    windows/
        x64/
        x86/
```

and add to the packaging command:

```text
/p:BuiltOnCI=true
```

related CI scripts:

- `BuiltOnCI` var:
  - [https://github.com/51Degrees/common-ci/blob/main/dotnet/build-project-core.ps1]
  - [https://github.com/51Degrees/common-ci/blob/main/dotnet/build-package-nuget.ps1]
  - [https://github.com/51Degrees/common-ci/blob/main/dotnet/build-project-framework.ps1]
  - [https://github.com/51Degrees/device-detection-dotnet/blob/main/ci/run-performance-tests-console.ps1]
- Copying native binaries:
  - [https://github.com/51Degrees/device-detection-dotnet/blob/main/ci/build-package.ps1]

#### Strong naming

All NuGet packages published from this repository are [strong-name](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/strong-naming) signed so they can be referenced by strong-named consumer applications.

Local developer builds use the [PublicSign](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/security#publicsign) pattern with the committed `51Degrees.publickey` (no secrets required). Production builds run in CI and use the matching private key from a GitHub organization secret to produce a real cryptographic signature.

See the [Strong naming section in the pipeline-dotnet README](https://github.com/51Degrees/pipeline-dotnet#strong-naming) for the full mechanics — the same setup is applied across every 51Degrees .NET shipping repo so the dependency chain remains consistent under strong-name identity checks.

##### Upgrading from non-strong-named to strong-named packages

Strong-name signing changes the assembly identity (it now carries a public key token), so when upgrading from an older unsigned version:

- Recompile anything that references the 51Degrees/Pipeline assemblies directly. Binding redirects cannot bridge unsigned to signed; only a rebuild can.
- Upgrade all 51Degrees packages to matching versions, then add binding redirects to reconcile versions across the signed assemblies (auto-generated with `PackageReference`; with `packages.config` enable `AutoGenerateBindingRedirects` or run `Add-BindingRedirect`).
- Update any hardcoded assembly-qualified names (`Assembly.Load`, `Type.GetType`, `.config`) to include the new public key token.
- Do a clean rebuild: clear `bin`/`obj` and remove any stale unsigned 51Degrees DLLs from output/deploy folders.

For a working .NET Framework reference, see the [Cloud/Framework-Web example](https://github.com/51Degrees/device-detection-dotnet-examples/blob/main/Examples/Cloud/Framework-Web/Web.config), whose `Web.config` already contains the full `<assemblyBinding>` redirect block.

## Examples

Examples can be found in
[device-detection-dotnet-examples](https://github.com/51Degrees/device-detection-dotnet-examples)
repository.

## Tests

Tests can be found in the `Tests/` folder. These can all be run from within
Visual Studio or by using the `dotnet test` command line tool.

Some tests require additional resources to run. These will either fail or return
an 'inconclusive' result if these resources are not provided.

-   Some tests require an 'Enterprise' data file. This can be obtained by
    [purchasing a license](https://51degrees.com/pricing?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=tests).
    -   Once available, place the data file within the repository folder
        structure. The tests locate it by searching the folder hierarchy for
        the expected file name.
-   Tests using the cloud service require resource keys with specific properties
    to be provided using environment variables:
    -   The `_51DEGREES_RESOURCE_KEY` environment variable should be populated
        with a key that includes all properties. The legacy `SUPER_RESOURCE_KEY`
        environment variable is still supported and is checked second. A
        [license](https://51degrees.com/pricing?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=tests-2) is required in order to access
        some properties.

## Project documentation

For complete documentation on the Pipeline API and associated engines, see the
[51Degrees documentation site](https://51degrees.com/documentation/index.html?utm_source=github&utm_medium=readme&utm_campaign=device-detection-dotnet&utm_content=readme.md&utm_term=project-documentation).
