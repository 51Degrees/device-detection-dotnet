using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// @example OnPremise/Framework-Web/Default.aspx.cs
/// 
/// This example demonstrates how to use the cloud-based device detection API in a .NET Framework 
/// website.
/// 
/// The source code for this example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/tree/master/Examples/OnPremise/Framework-Web).
/// 
/// @include{doc} example-require-datafile.txt
/// 
/// Required NuGet Dependencies: 
/// - [FiftyOne.DeviceDetection](https://www.nuget.org/packages/FiftyOne.DeviceDetection/)
/// - [FiftyOne.Pipeline.Web](https://www.nuget.org/packages/FiftyOne.Pipeline.Web/)
/// 
/// ## Overview
/// 
/// The FiftyOne.Pipeline.Web package includes an IHttpModule implementation called 
/// 'PipelineModule'. This is inserted into the start of the ASP.NET processing pipeline so
/// that it can intercept the request and perform device detection.
/// 
/// The module replaces the default `HttpCapabilitiesBase.BrowserCapabilitiesProvider` with a 
/// 51Degrees version.
/// This means that when values are requested (e.g. Request.Browser.IsMobileDevice), we can 
/// intercept that request and supply a result from our device detection.
/// 
/// If you require access to properties and features that are not available through 
/// `BrowserCapabilitiesProvider`, you will need to cast browser capabilities to our 
/// `PipelineCapabilities` class in order to access the `IFlowData` instance directly:
/// 
/// ```
/// var flowData = ((PipelineCapabilities)Request.Browser).FlowData;
/// var deviceData = flowData.Get<IDeviceData>();
/// ```
/// 
/// The 'IPipeline' instance that is being used can also be accessed through the static 
/// `WebPipeline` class:
/// 
/// ```
/// var deviceDetectionPipeline = WebPipeline.GetInstance();
/// ```
/// 
/// ## Configuration
/// 
/// By default, a 51Degrees.json file is used to supply the Pipeline configuration.
/// For more details about the available options, check the relevant builder classes. 
/// For example, the CloudRequestEngineBuilder. The methods available on the builder are 
/// the same as those that will be available in the configuration file. 
/// 
/// Note that you will need to update the configuration with the complete path to a data file. 
/// The free 'lite' file is included with this repository under the path 
/// `FiftyOne.DeviceDetection\device-detection-cxx\device-detection-data`
/// Alternatively, you can obtain a [license key](http://51degrees.com/pricing) which can be used 
/// to download a data file from our 
/// [Distributor](https://51degrees.com/documentation/_info__distributor.html) service.
/// 
/// @include OnPremise/Framework-Web/App_Data/51Degrees.json
/// 
/// ### Web.config
/// 
/// The 51Degrees API mostly targets .NET Standard. This means you may get an error like:
/// 
/// ```
/// CS0012: The type 'System.Object' is defined in an assembly that is not referenced. You must add 
/// a reference to assembly 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'.
/// ```
/// 
/// In this case, you will need to add a section similar to the following to your web.config:
/// 
/// ```
/// <compilation debug="true" targetFramework="4.7.2">
///   <assemblies>
///     <add assembly="netstandard, Version=2.0.0.0, Culture=neutral, 
///           PublicKeyToken=cc7b13ffcd2ddd51"/>
///   </assemblies>
/// </compilation>
/// ```
/// 
/// Another common problem is an error loading a specific version of a library because your
/// project is using a newer version. For example, this message is appearing because our API only 
/// requires version 11 of the Newtonsoft library, but the sample project is using version 13:
/// 
/// ```
/// Could not load file or assembly 'Newtonsoft.Json, Version=11.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference. (Exception from HRESULT: 0x80131040)
/// ```
/// 
/// Fixing this requires a binding redirect to tell the runtime to use the installed version, 
/// rather than the requested version.
/// For example, to direct it to use version 13 of the Newtonsoft library regardless of the 
/// requested version, we would add:
/// 
/// ```
/// <runtime>
///   <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
///     <dependentAssembly>
///       <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" />
///       <bindingRedirect oldVersion="0.0.0.0-13.0.0.0" newVersion="13.0.0.0" />
///     </dependentAssembly>
///   </assemblyBinding>
/// </runtime>
/// ```
/// 
/// ## Load Assemblies
/// 
/// Any builders that are specified in configuration must also have their assemblies loaded into
/// the AppDomain. This is handled in Global.asax:
/// 
/// @include OnPremise/Framework-Web/Global.asax.cs
/// 
/// ## Results
/// 
/// This example includes a simple demonstration page that shows how to access different values 
/// from the results. This page also demonstrates how to use client-side evidence to determine 
/// the model of Apple devices.
/// 
/// @include OnPremise/Framework-Web/Default.aspx
namespace Framework_Web
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}