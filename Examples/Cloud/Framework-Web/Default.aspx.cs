using System;
using System.Web.UI;

/// @example Cloud/Framework-Web/Default.aspx.cs
/// 
/// This example demonstrates how to use the cloud-based device detection API in a .NET Framework 
/// website.
/// 
/// The source code for this example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/tree/master/Examples/Cloud/Framework-Web).
/// 
/// ## Overview
/// 
/// The FiftyOne.Pipeline.Web package includes an IHttpModule implementation called 
/// 'PipelineModule', which replaces the default HttpCapabilitiesBase.BrowserCapabilitiesProvider
/// with a 51Degrees version.
/// This means that when values are requested (e.g. Request.Browser.IsMobileDevice), we can 
/// intercept that request and perform our own device detection based on the values in the 
/// HTTPRequest. 
/// 
/// ## Configuration
/// 
/// By default, a 51Degrees.json file is used to supply the Pipeline configuration.
/// For more details about the available options, check the relevant builder classes. 
/// For example, the CloudRequestEngineBuilder. The methods available on the builder are 
/// the same as those that will be available in the configuration file. 
/// 
/// Note that you will need to create a 'resource key' using our 
/// [configurator](https://configure.51degrees.com) site in order to get this example to work.
/// See our [documentation](http://51degrees.com/documentation/4.4/_concepts__configurator.html) 
/// for complete instructions. Once created, the key will need to be copied into this 
/// configuration file.
/// 
/// @include Cloud/Framework-Web/App_Data/51Degrees.json
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
/// In this case, you will need to add the following section to your web.config:
/// 
/// ```{cs}
/// <compilation debug="true" targetFramework="4.7.2">
///   <assemblies>
///     <add assembly="netstandard, Version=2.0.0.0, Culture=neutral, 
///           PublicKeyToken=cc7b13ffcd2ddd51"/>
///   </assemblies>
/// </compilation>
/// ```
/// 
/// ## Load Assemblies
/// 
/// Any builders that are specified in configuration must also have their assemblies loaded into
/// the AppDomain. This is handled in Global.asax:
/// 
/// @include Cloud/Framework-Web/Global.asax
/// 
/// ## Results
/// 
/// This example includes a simple demonstration page that shows how to access different values 
/// from the results. For a complete list of the properties available, see our 
/// [Configurator](https://configure.51degrees.com) tool. This is also used to create the resource 
/// keys that are required when accessing our cloud service.
/// 
/// @include Cloud/Framework-Web/Default.aspx
namespace Framework_Web
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}