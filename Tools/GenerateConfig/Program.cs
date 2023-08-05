using FiftyOne.DeviceDetection.Apple;
using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Uach;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.JavaScriptBuilder.FlowElement;
using FiftyOne.Pipeline.JsonBuilder.FlowElement;
using FiftyOne.Pipeline.Web.Shared;
using GenerateConfig;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

var writerOptions = new JsonWriterOptions
{
    Indented = true
};

// builder we want to appear in a specific order at the start of the file.
var firstBuilders = new Type[]
{
    typeof(SequenceElementBuilder),
    typeof(ShareUsageBuilder),
    typeof(AppleProfileEngineBuilder),
    typeof(UachJsConversionElementBuilder),
    typeof(CloudRequestEngineBuilder)
};
// builder we want to appear in a specific order at the end of the file.
var lastBuilders = new Type[]
{
    typeof(JsonBuilderElementBuilder),
    typeof(JavaScriptBuilderElementBuilder),
    typeof(SetHeadersElementBuilder)
};

// This isn't actually used for anything other than ensuring the relevant types are
// loaded into the AppDomain so that reflection can find them.
var middleBuilders = new Type[]
{
    typeof(DeviceDetectionCloudEngineBuilder),
    typeof(HardwareProfileCloudEngineBuilder),
    typeof(DeviceDetectionHashEngineBuilder),
};

using (FileStream fs = File.Create("output.json"))
using (var writer = new Utf8JsonWriter(fs, writerOptions))
{
    writer.WriteStartObject();

    writer.WriteCommentValue("This file demonstrates the options available when using a configruation file.\r\nThe comments contain the location in the code to look for information on what the property does.\r\nEach property value is set to the default for that property.");

    writer.WritePropertyName(nameof(PipelineOptions));
    writer.WriteStartObject();
    writer.WritePropertyName(nameof(PipelineOptions.Elements));
    writer.WriteStartArray();

    // Add an entry for each builder
    foreach (var builder in firstBuilders)
    {
        Helpers.AppendConfigForElementBuilder(writer, builder);
    }
    foreach (var builder in Helpers.GetAvailableElementBuilders()
        .Where(b => firstBuilders.Contains(b) == false && lastBuilders.Contains(b) == false))
    {
        Helpers.AppendConfigForElementBuilder(writer, builder);
    }
    foreach (var builder in lastBuilders)
    {
        Helpers.AppendConfigForElementBuilder(writer, builder);
    }
    writer.WriteEndArray();

    // Add pipeline builder options.
    // FiftyOnePipelineBuilder is the default builder used by the web integration, so we're using that one here.
    Helpers.AppendConfigForPipelineBuilder(writer, typeof(FiftyOnePipelineBuilder));

    // Add web integration options.
    var options = new PipelineWebIntegrationOptions();

    var optionsType = typeof(PipelineWebIntegrationOptions);

    var propertyName = nameof(PipelineWebIntegrationOptions.ClientSideEvidenceEnabled);
    writer.WriteCommentValue($"See {optionsType.FullName}.{propertyName}");
    writer.WriteBoolean(propertyName, options.ClientSideEvidenceEnabled);

    propertyName = nameof(PipelineWebIntegrationOptions.UseAsyncScript);
    writer.WriteCommentValue($"See {optionsType.FullName}.{propertyName}");
    writer.WriteBoolean(propertyName, options.UseAsyncScript);

    propertyName = nameof(PipelineWebIntegrationOptions.UseSetHeaderProperties);
    writer.WriteCommentValue($"See {optionsType.FullName}.{propertyName}");
    writer.WriteBoolean(propertyName, options.UseSetHeaderProperties);

    writer.WriteEndObject();
    writer.WriteEndObject();
}

