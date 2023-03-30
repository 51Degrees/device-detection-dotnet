using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.JavaScriptBuilder.FlowElement;
using FiftyOne.Pipeline.JsonBuilder.FlowElement;
using System;
using System.Web;
using System.Web.Routing;

namespace Framework_Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Make sure the assemblies that are needed by the pipeline are loaded into the
            // app domain.
            // This is needed in order from BuildFromConfiguration to be able to find the
            // relevant builder types when using reflection.
            AppDomain.CurrentDomain.Load(typeof(DeviceDetectionHashEngine).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(JavaScriptBuilderElement).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(JsonBuilderElement).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(SequenceElementBuilder).Assembly.GetName());
        }
    }
}