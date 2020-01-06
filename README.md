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
  - *FiftyOne.DeviceDetection.Hash.** - 
  - *FiftyOne.DeviceDetection.Pattern.** - 
  - *FiftyOne.DeviceDetection.Shared* - 
  - *FiftyOne.DeviceDetection* - 
  - *FiftyOne.DeviceDetection.Cloud* - 

## Examples

### Device Detection
TBC

## Project documentation

For complete documentation on the Pipeline API and associated engines, see the [51Degrees documentation site][Documenation].

## Enable debugging of NuGet packages

In order to debug into NuGet packages, you must be using packages that reference debug symbols. By default, this includes all pre-release packages but not final versions.
If you have a debuggable package then you will need to configure Visual Studio to allow you to step into it:

- In tools -> options -> debugging -> symbols, add the Azure DevOps symbol server: 
![Visual Studio 2017 screenshot with symbol server added][ImageAddSymbolServer]
- Select the ‘Load only specified modules’ option at the bottom and configure it to only load Symbols for 51Degrees modules as shown below:
![Visual Studio 2017 configured to only load external symbols for 51Degrees libraries][ImageLoadOnlyFiftyone]
- In tools -> options -> debugging -> general, ensure that:
  - Enable Just My Code is off. Having this on will prevent VS stepping into any NuGet packages.
  - Enable source server support is on.
  - Example Source Link support is on.
![Visual Studio 2017 configured for debugging external packages][ImageConfigureDebugger]

When stepping into a method from a relevant NuGet package, you should now see the following warning message:
![Visual Studio 2017 Source Link download warning][ImageSourceLinkDownload]


[Documentation]: https://docs.51degrees.com
[ImageAddSymbolServer]: file://Images/vs2017-add-symbol-server.png
[ImageConfigureDebugger]: file://Images/vs2017-configure-debugger.png
[ImageLoadOnlyFiftyone]: file://Images/vs2017-load-only-fiftyone.png
[ImageSourceLinkDownload]: file://Images/vs2017-source-link-download.png