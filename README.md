# 51Degrees Device Detection Engines

![51Degrees](https://51degrees.com/DesktopModules/FiftyOne/Distributor/Logo.ashx?utm_source=github&utm_medium=repository&utm_content=readme_main&utm_campaign=dotnet-open-source)
**Pipeline API**

[Developer
Documentation](https://51degrees.com/device-detection-dotnet/index.html?utm_source=github&utm_medium=repository&utm_content=documentation&utm_campaign=dotnet-open-source)

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
[dependencies](https://51degrees.com/documentation/_info__dependencies.html)
page. The `ci/options.json` file lists the tested and packaged .NET versions
and operating systems automatic tests are performed with. The solution will
likely operate with other versions.

### Data

The API can either use our cloud service to get its data or it can use a local
(on-premise) copy of the data.

#### Cloud

You will require a [resource
key](https://51degrees.com/documentation/_info__resource_keys.html) to use the
Cloud API. You can create resource keys using our
[configurator](https://configure.51degrees.com/), see our
[documentation](https://51degrees.com/documentation/_concepts__configurator.html)
on how to use this.

#### On-Premise

In order to perform device detection on-premise, you will need to use a
51Degrees data file. This repository includes a free, 'lite' file in the
'device-detection-data' sub-module that has a significantly reduced set of
properties. To obtain a file with a more complete set of device properties see
the [51Degrees website](https://51degrees.com/pricing). If you want to use the
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
    [purchasing a license](https://51degrees.com/pricing).
    -   Once available, the full path to this data file must be specified in the
        `DEVICEDETECTIONDATAFILE` environment variable.
-   Tests using the cloud service require resource keys with specific properties
    to be provided using environment variables:
    -   The `SUPER_RESOURCE_KEY` environment variable should be populated with a
        key that includes all properties. A
        [license](https://51degrees.com/pricing) is required in order to access
        some properties.

## Project documentation

For complete documentation on the Pipeline API and associated engines, see the
[51Degrees documentation site](https://51degrees.com/documentation/index.html).
