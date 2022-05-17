# 51Degrees Device Detection Engines

![51Degrees](https://51degrees.com/DesktopModules/FiftyOne/Distributor/Logo.ashx?utm_source=github&utm_medium=repository&utm_content=readme_main&utm_campaign=dotnet-open-source "Data rewards the curious") **Pipeline API**

[Developer Documentation](https://51degrees.com/device-detection-dotnet/index.html?utm_source=github&utm_medium=repository&utm_content=documentation&utm_campaign=dotnet-open-source "developer documentation")

## Introduction

This repository contains the device detection engines for the .NET implementation of the Pipeline 
API.

## Dependencies

Visual Studio 2019 or later is recommended. Although Visual Studio Code can be used for working 
with most of the projects.

The core device detection projects are written in C and C++.
The Pipeline engines are written in C# and target .NET Standard 2.0.3.
Example and test projects mostly target .NET Core 3.1 though in some cases, projects are available 
targeting other frameworks.

For runtime dependencies, see our [dependencies](http://51degrees.com/documentation/_info__dependencies.html) page.
The [tested versions](https://51degrees.com/documentation/_info__tested_versions.html) page shows 
the .NET versions that we currently test against. The software may run fine against other versions, 
but additional caution should be applied.

### Data file

In order to perform device detection on-premise, you will need to use a 51Degrees data file. 
(This is not required for cloud users) This repository includes a free, 'lite' file in the 
'device-detection-data' sub-module that has a significantly reduced set of properties. To obtain 
a file with a more complete set of device properties see the 
[51Degrees website](https://51degrees.com/pricing). If you want to use the lite file, you will 
need to install [GitLFS](https://git-lfs.github.com/):

```
sudo apt-get install git-lfs
git lfs install
```

Then, navigate to 'FiftyOne.DeviceDetection/device-detection-cxx/device-detection-data' and execute:

```
git lfs pull
```

## Solutions and projects

- **FiftyOne.DeviceDetection** - Device detection engines and related projects.
  - **FiftyOne.DeviceDetection.Hash** - .NET implementation of the device detection hash engine.
  - **FiftyOne.DeviceDetection.Shared** - Shared classes used by the device detection engines.
  - **FiftyOne.DeviceDetection** - Contains device detection engine builders.
  - **FiftyOne.DeviceDetection.Cloud** - A .NET engine which retrieves device detection results by 
  consuming the 51Degrees cloud service. This can be swapped out with either the hash or pattern 
  engines seamlessly.
  
## Installation

### Nuget

The easiest way to install is to use NuGet to add the reference to the package:

```
Install-Package FiftyOne.DeviceDetection
```

### Build from Source

Device detection on-premise uses a native binary. (i.e. compiled from C code to target a specific 
platform/architecture) The NuGet package contains several binaries for common platforms. 
However, in some cases, you'll need to build the native binaries yourself for your target platform. 
This section explains how to do this.

#### Pre-requisites

- Install C build tools:
  - Windows:
    - You will need either Visual Studio 2019 or the [C++ Build Tools](https://visualstudio.microsoft.com/visual-cpp-build-tools/) installed.
      - Minimum platform toolset version is `v142`
      - Minimum Windows SDK version is `10.0.18362.0`
  - Linux/MacOS:
    - `sudo apt-get install g++ make libatomic1`
- If you have not already done so, pull the git submodules that contain the native code:
  - `git submodule update --init --recursive`

Visual studio should now be able to build the native binaries as part of it's normal build process.

## Examples

The tables below describe the examples available in this repository.

### Cloud

| Example                                | Description |
|----------------------------------------|-------------|
| GettingStarted-Console                 | How to use the 51Degrees Cloud service to determine details about a device based on its User-Agent and User-Agent Client Hints HTTP header values. |
| GettingStarted-Web                     | How to use the 51Degrees Cloud service to determine details about a device as part of a simple ASP.NET website. |
| Metadata                               | How to access the meta-data that relates to things like the properties populated device detection |
| TacLookup                              | How to get device details from a TAC (Type Allocation Code) using the 51Degrees cloud service. |
| NativeModelLookup                      | How to get device details from a native model name using the 51Degrees cloud service. |
| GetAllProperties                       | How to iterate through all properties available in the cloud response. |
| ClientHints NetCore 3.1                | Legacy example. Retained for the associated automated tests. See GettingStarted-Web instead. |
| ClientHints Not Integrated NetCore 3.1 | Legacy example. Our ASP.NET integration will automatically set the `Accept-CH` header that is used to request User-Agent Client Hints headers.This example demonstrates how to do this without using the integration component. |

### On-Premise

| Example                                | Description |
|----------------------------------------|-------------|
| GettingStarted-Console                 | How to use the 51Degrees on-premise device detection API to determine details about a device based on its User-Agent and User-Agent Client Hints HTTP header values. |
| GettingStarted-Web                     | How to use the 51Degrees Cloud service to determine details about a device as part of a simple ASP.NET website. |
| Metadata                               | How to access the meta-data that relates to things like the properties populated device detection |
| offline_processing                     | Example showing how to ingest a file containing data from web requests and perform detection against the entries. |
| Performance                            | How to configure the various performance options and run a simple performance test. |
| UpdateOnStartup                        | How to configure the Pipeline to automatically update the device detection data file on startup. |
| UpdatePollingInterval                  | Ho to configure and verify the various automatic data file update settings. |
| ClientHints NetCore 3.1                | Legacy example. Retained for the associated automated tests. See GettingStarted-Web instead. |
| ClientHints Not Integrated NetCore 3.1 | Legacy example. Our ASP.NET integration will automatically set the `Accept-CH` header that is used to request User-Agent Client Hints headers. This example demonstrates how to do this without using the integration component. |

## Tests

Tests can be found in the `Tests/` folder. These can all be run from within Visual Studio or by 
using the `dotnet test` command line tool. 

## Project documentation

For complete documentation on the Pipeline API and associated engines, see the 
[51Degrees documentation site][Documentation].

[Documentation]: https://51degrees.com/documentation/index.html
[nuget]: https://www.nuget.org/packages/FiftyOne.DeviceDetection/
