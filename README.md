# 51Degrees Device Detection Engines

![51Degrees](https://51degrees.com/DesktopModules/FiftyOne/Distributor/Logo.ashx?utm_source=github&utm_medium=repository&utm_content=readme_main&utm_campaign=dotnet-open-source "Data rewards the curious") **Pipeline API**

[Developer Documentation](https://51degrees.com/device-detection-dotnet/index.html?utm_source=github&utm_medium=repository&utm_content=documentation&utm_campaign=dotnet-open-source "developer documentation")

## Introduction

This repository contains the device detection engines for the .NET implementation of the Pipeline API.

## Pre-requesites

Visual Studio 2019 or later is recommended. Although Visual Studio Code can be used for working with most of the projects.

The core device detection projects are written in C and C++.
The Pipeline engines are written in C# and target .NET Standard 2.0.3.
Example and test projects mostly target .NET Core 3.1 though in some cases, projects are available targeting other frameworks.

## Solutions and projects

- **FiftyOne.DeviceDetection** - Device detection engines and related projects.
  - **FiftyOne.DeviceDetection.Hash** - .NET implementation of the device detection hash engine.
  - **FiftyOne.DeviceDetection.Shared** - Shared classes used by the device detection engines.
  - **FiftyOne.DeviceDetection** - Contains device detection engine builders.
  - **FiftyOne.DeviceDetection.Cloud** - A .NET engine which retrieves device detection results by consuming the 51Degrees cloud service. This can be swapped out with either the hash or pattern engines seamlessly.
  
## Installation

You can either reference the projects in this repository or you can reference the [NuGet][nuget] packages in your project:

```
Install-Package FiftyOne.DeviceDetection
```

## Examples

Examples can be found in the `Examples/` folder, there are separate sources for cloud, hash and pattern implementations and solutions for .NET Core and .NET Framework. See below for a list of examples.

### Device Detection

|Example|Description|Implementations|
|-------|-----------|---------------|
|ConfigureFromFile|This example shows how to build a Pipeline from a configuration file.|On-premise|
|GettingStarted|This example uses 51Degrees device detection to determine whether a given User-Agent corresponds to a mobile device or not.|On-premise / Cloud|
|Metadata|This example shows how to get all the properties from the device detection engine and print their information details.|On-premise|
|Performance|This example demonstrates the performance of the maximum performance device detection configuration profile.|On-premise|
|NativeModelLookup|This example demonstrates how to get device details from a native model name using the 51Degrees cloud service.|Cloud|
|TacLookup|This example demonstrates how to get device details from a TAC (Type Allocation Code) using the 51Degrees cloud service.|Cloud|
|GetAllProperties|This example demonstrates how to iterate through all properties in a response.|Cloud|

### Web Integrations

These examples show how to integrate the Pipeline API into a simple web app.

|Example|Description|
|-------|-----------|
|Cloud DeviceDetectionWebDemo NetCore 3.1|This example demonstrates how to use the 51Degrees cloud to perform device detection from an ASP.NET Core 3.1 web application|
|DeviceDetectionWebDemo NetCore 2.1|This example demonstrates how to use on-premise device detection from an ASP.NET Core 2.1 web application|
|DeviceDetectionWebDemo NetCore 3.1|This example demonstrates how to use on-premise device detection from an ASP.NET Core 3.1 web application|

## Tests

Tests can be found in the `Tests/` folder. These can all be run from within Visual Studio or by using the `dotnet` command line tool. 

## Project documentation

For complete documentation on the Pipeline API and associated engines, see the [51Degrees documentation site][Documentation].

[Documentation]: https://51degrees.com/documentation/index.html
[nuget]: https://www.nuget.org/packages/FiftyOne.DeviceDetection/
