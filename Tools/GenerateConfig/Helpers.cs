using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GenerateConfig
{
    internal static class Helpers
    {
        /// <summary>
        /// Use reflection to get all element builders.
        /// These are defined as any type that has a Build method 
        /// where the return type is or implements IFlowElement.
        /// These will be used when building a pipeline from configuration.
        /// </summary>
        public static List<Type> GetAvailableElementBuilders()
        {
            var builders = new List<Type>();
            // Get all loaded types
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()
#if DEBUG
                // Exclude VisualStudio assemblies
                .Where(a => a.FullName != null && 
                    a.FullName.StartsWith("Microsoft.VisualStudio",
                        StringComparison.OrdinalIgnoreCase) == false)
#endif
                )
            {
                // Exclude dynamic assemblies
                if (assembly.IsDynamic == false)
                {
                    // Get all types that have..
                    builders.AddRange(assembly.GetTypes()
                        .Where(t => t.IsAbstract == false && t.GetMethods()
                            // ..a method called 'Build'..
                            .Any(m => m.Name == "Build" &&
                            // ..where the return type is or implements IFlowElement
                            (m.ReturnType == typeof(IFlowElement) ||
                            m.ReturnType.GetInterfaces().Contains(typeof(IFlowElement)))))
                    .ToList());
                }
            }

            return builders;
        }

        public static void AppendConfigForElementBuilder(Utf8JsonWriter writer, Type builder)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(ElementOptions.BuilderName), builder.Name);

            writer.WritePropertyName(nameof(ElementOptions.BuildParameters));
            writer.WriteStartObject();

            AppendPropertiesFromSetMethods(writer, builder);
            AppendPropertiesFromBuildMethods(writer, builder);

            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        public static void AppendConfigForPipelineBuilder(Utf8JsonWriter writer, Type builder)
        {
            writer.WritePropertyName(nameof(PipelineOptions.PipelineBuilderParameters));
            writer.WriteStartObject();

            AppendPropertiesFromSetMethods(writer, builder);
            AppendPropertiesFromBuildMethods(writer, builder);

            writer.WriteEndObject();
        }

        private static void AppendPropertiesFromSetMethods(Utf8JsonWriter writer, Type builder)
        {
            // TODO - add defaults and comments
            var setMethods = builder.GetMethods().Where(m => m.GetParameters().Length == 1 && m.Name.StartsWith("Set"));
            foreach (var method in setMethods)
            {
                var type = method.GetParameters()[0].ParameterType;
                if (type == typeof(string))
                {
                    writer.WriteString(method.Name, "");
                }
                else if (type == typeof(bool))
                {
                    writer.WriteBoolean(method.Name, false);
                }
                else if (type == typeof(int))
                {
                    writer.WriteNumber(method.Name, 0);
                }
                else
                {
                    writer.WriteString(method.Name, $"Unexpected type - {type.Name}");
                }
            }
        }
        private static void AppendPropertiesFromBuildMethods(Utf8JsonWriter writer, Type builder)
        {
            // TODO - add defaults and comments
            var buildMethods = builder.GetMethods().Where(m => m.Name == "Build");
            foreach (var method in buildMethods)
            {
                foreach (var parameter in method.GetParameters())
                {
                    writer.WriteString(parameter.Name ?? "NULL", "");
                }
            }
        }
    }
}
