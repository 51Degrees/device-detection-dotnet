# 51Degrees Device Detection Engines

![51Degrees](https://51degrees.com/DesktopModules/FiftyOne/Distributor/Logo.ashx?utm_source=github&utm_medium=repository&utm_content=readme_main&utm_campaign=dotnet-open-source "Data rewards the curious") **Pipeline API**

[Developer Documentation](https://51degrees.com/device-detection-dotnet/index.html?utm_source=github&utm_medium=repository&utm_content=documentation&utm_campaign=dotnet-open-source "developer documentation")

## Introduction

This repository contains the device detection engines for the .NET implementation of the Pipeline 
API.

## Pre-requesites

Visual Studio 2019 or later is recommended. Although Visual Studio Code can be used for working 
with most of the projects.

The core device detection projects are written in C and C++.
The Pipeline engines are written in C# and target .NET Standard 2.0.3.
Example and test projects mostly target .NET Core 3.1 though in some cases, projects are available 
targeting other frameworks.

To build C/C++ projects:
- Minimum Platform Toolset Version `v142`
- Minimum Windows 10 SDK `10.0.18362.0`

## Solutions and projects

- **FiftyOne.DeviceDetection** - Device detection engines and related projects.
  - **FiftyOne.DeviceDetection.Hash** - .NET implementation of the device detection hash engine.
  - **FiftyOne.DeviceDetection.Shared** - Shared classes used by the device detection engines.
  - **FiftyOne.DeviceDetection** - Contains device detection engine builders.
  - **FiftyOne.DeviceDetection.Cloud** - A .NET engine which retrieves device detection results by 
  consuming the 51Degrees cloud service. This can be swapped out with either the hash or pattern 
  engines seamlessly.
  
## Installation

You can either reference the projects in this repository or you can reference the [NuGet][nuget] 
packages in your project:

```
Install-Package FiftyOne.DeviceDetection
```

On Windows, make sure to install `C++ Redistributable 4.2* latest or above`. This is required to 
use the native binaries shipped with the NuGet package.

## Examples

We are currently in the process of revamping the examples, so they are a little disorganized.
The new-style examples can be found in the `Examples/` folder, while legacy examples are mainly in 
`FiftyOne.DeviceDetection/Examples/`.

The revamped examples have more consistent naming and organization, comments that are clearer and 
explain things in more detail and improved handling of common failure scenarios. We will be
revamping more of the examples over the coming releases.

See the tables below for details on the examples that are available.

### Device Detection

#### Cloud

| Example                                | Revamped           | Description |
|----------------------------------------|--------------------|-------------|
| GettingStarted-Console                 | @tick              | How to use the 51Degrees Cloud service to determine details about a device based on its User-Agent and User-Agent Client Hints HTTP header values. |
| GettingStarted-Web                     | @tick              | How to use the 51Degrees Cloud service to determine details about a device as part of a simple ASP.NET website. |
| TacLookup                              |                    | How to get device details from a TAC (Type Allocation Code) using the 51Degrees cloud service. |
| NativeModelLookup                      |                    | How to get device details from a native model name using the 51Degrees cloud service. |
| GetAllProperties                       |                    | How to iterate through all properties available in the cloud response. |
| ClientHints NetCore 3.1                |                    | Legacy example. Retained for the associated automated tests. See GettingStarted-Web instead. |
| ClientHints Not Integrated NetCore 3.1 |                    | Our ASP.NET integration will automatically set the `Accept-CH` header that is used to request User-Agent Client Hints headers.This example demonstrates how to do this without using the integration component. |

#### On-Premise

| Example                                | Revamped           | Description |
|----------------------------------------|--------------------|-------------|
| GettingStarted-Console                 | @tick              | How to use the 51Degrees on-premise device detection API to determine details about a device based on its User-Agent and User-Agent Client Hints HTTP header values. |
| GettingStarted-Web                     | @tick              | How to use the 51Degrees Cloud service to determine details about a device as part of a simple ASP.NET website. |
| Metadata                               |                    | How to access the meta-data that relates to the device detection algorithm. |
| Performance                            |                    | How to configure the various performance options and run a simple performance test. |
| UpdateOnStartup                        |                    | How to configure the Pipeline to automatically update the device detection data file on startup. |
| UpdatePollingInterval                  |                    | Ho to configure and verify the various automatic data file update settings. |
| ClientHints NetCore 3.1                |                    | Legacy example. Retained for the associated automated tests. See GettingStarted-Web instead. |
| ClientHints Not Integrated NetCore 3.1 |                    | Our ASP.NET integration will automatically set the `Accept-CH` header that is used to request User-Agent Client Hints headers. This example demonstrates how to do this without using the integration component. |

## Tests

Tests can be found in the `Tests/` folder. These can all be run from within Visual Studio or by 
using the `dotnet` command line tool. 

## Project documentation

For complete documentation on the Pipeline API and associated engines, see the 
[51Degrees documentation site][Documentation].

[Documentation]: https://51degrees.com/documentation/index.html
[nuget]: https://www.nuget.org/packages/FiftyOne.DeviceDetection/
