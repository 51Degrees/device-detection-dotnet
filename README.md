# 51Degrees Device Detection Engines

![51Degrees](https://51degrees.com/DesktopModules/FiftyOne/Distributor/Logo.ashx?utm_source=github&utm_medium=repository&utm_content=readme_main&utm_campaign=dotnet-open-source "Data rewards the curious") **Pipeline API**

[Developer Documentation](https://docs.51degrees.com?utm_source=github&utm_medium=repository&utm_content=documentation&utm_campaign=dotnet-open-source "developer documentation")

## Introduction

This repository contains the device detection engines for the .NET implementation of the Pipeline API.

## Pre-requesites

Visual Studio 2017 or later is recommended. Although Visual Studio Code can be used for working with most of the projects.

The core device detection projects are written in C and C++.
The Pipeline engines are written in C# and target .NET Standard 2.0.3 and .NET Core 2.0.

## Solutions and projects

- **FiftyOne.DeviceDetection** - Device detection engines and related projects.
  - **FiftyOne.DeviceDetection.Hash.** - .NET implementation of the device detection hash engine.
  - **FiftyOne.DeviceDetection.Pattern.** - .NET implementation of the device detection pattern engine.
  - **FiftyOne.DeviceDetection.Shared** - Shared classes used by the device detection engines.
  - **FiftyOne.DeviceDetection** - Contains device detection engine builders.
  - **FiftyOne.DeviceDetection.Cloud** - A .NET engine which retrieves device detection results by consuming the 51Degrees cloud service. This can be swapped out with either the hash or pattern engines seamlessly.
  
## Installation

You can either reference the projects in this repository or you can reference the [NuGet][nuget] packages in your project:

```
Install-Package FiftyOne.DeviceDetection -Version 4.1.0
```

Make sure to select the latest version from [NuGet.][nuget]
## Examples

Examples can be found in the `Examples/` folder, there are separate sources for cloud, hash and pattern implementations and solutions for .NET Core and .NET Framework. See below for a list of examples.

### Device Detection

|Example|Description|Runtime|Algortihm|
|-------|-----------|-------|---------|
|ConfigureFromFile|This example shows how to build a Pipeline from a configuration file.|Core / Framework|Hash / Pattern|
|GettingStarted|This example uses 51Degrees device detection to determine whether a given User-Agent corresponds to a mobile device or not.|Core / Framework|Cloud / Hash / Pattern|
|Metadata|This example shows how to get all the properties from the device detection engine and print their information details.|Core / Framework|Hash / Pattern|
|Performance|This example demonstrates the performance of the maximum performance device detection configuration profile.|Core / Framework|Hash / Pattern|

### DeviceDetectionWebDemoV4

This example shows how to integrate the Pipeline API with a device detection engine into an ASP.NET Core web app.

## Tests

Tests can be found in the `Tests/` folder. These can all be run from within Visual Studio.

## Project documentation

For complete documentation on the Pipeline API and associated engines, see the [51Degrees documentation site][Documentation].

[Documentation]: https://docs.51degrees.com
[nuget]: https://www.nuget.org/packages/FiftyOne.DeviceDetection/
