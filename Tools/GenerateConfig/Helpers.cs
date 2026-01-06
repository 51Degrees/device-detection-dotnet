/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2026 51 Degrees Mobile Experts Limited, Davidson House,
 * Forbury Square, Reading, Berkshire, United Kingdom RG1 3EU.
 *
 * This Original Work is licensed under the European Union Public Licence
 * (EUPL) v.1.2 and is subject to its terms as set out below.
 *
 * If a copy of the EUPL was not distributed with this file, You can obtain
 * one at https://opensource.org/licenses/EUPL-1.2.
 *
 * The 'Compatible Licences' set out in the Appendix to the EUPL (as may be
 * amended by the European Commission) shall be deemed incompatible for
 * the purposes of the Work and the provisions of the compatibility
 * clause in Article 5 of the EUPL shall not apply.
 *
 * If using the Work as, or as part of, a network application, by
 * including the attribution notice(s) required under Article 5 of the EUPL
 * in the end user terms of the application under an appropriate heading,
 * such notice(s) shall fulfill the requirements of that article.
 * ********************************************************************* */

using FiftyOne.Pipeline.Core.Attributes;
using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Core.FlowElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

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

        /// <summary>
        /// Write the configuration options for the specified element builder using the 
        /// specified json writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="builder"></param>
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

        /// <summary>
        /// Write the configuration options for the specified pipeline builder using the 
        /// specified json writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="builder"></param>
        public static void AppendConfigForPipelineBuilder(Utf8JsonWriter writer, Type builder)
        {
            writer.WritePropertyName(nameof(PipelineOptions.BuildParameters));
            writer.WriteStartObject();

            AppendPropertiesFromSetMethods(writer, builder);
            AppendPropertiesFromBuildMethods(writer, builder);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Write the configuration options available on 'Set' methods from the specified builder
        /// using the specified json writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="builder"></param>
        private static void AppendPropertiesFromSetMethods(Utf8JsonWriter writer, Type builder)
        {
            // Get methods on the builder where..
            var setMethods = builder.GetMethods().Where(m => 
                // .. there is only 1 parameter
                m.GetParameters().Length == 1 &&
                // .. and the method name starts with 'Set'
                m.Name.StartsWith("Set") &&
                // .. and it does not have the 'CodeConfigOnly' attribute
                m.GetCustomAttribute(typeof(CodeConfigOnlyAttribute)) == null);

            foreach (var method in setMethods)
            {
                // Get default value
                var defaultValueAttr = method.GetCustomAttribute(typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                var defaultValue = defaultValueAttr?.DefaultValue;
                if (defaultValue == null)
                {
                    defaultValue = "No default value";
                }

                // Write a comment about where to find out more about what this setting does.
                var methodParmaterType = method.GetParameters()[0].ParameterType;
                writer.WriteCommentValue($"See {method?.DeclaringType?.Namespace}.{method?.DeclaringType?.Name}" +
                    $".{method?.Name}({methodParmaterType.Name})");

                // Write the json property
                WriteParameterLine(writer, method?.Name ?? "NULL", defaultValue);
            }
        }

        /// <summary>
        /// Write the configuration parameters available on 'Build' methods from the specified builder
        /// using the specified json writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="builder"></param>
        private static void AppendPropertiesFromBuildMethods(Utf8JsonWriter writer, Type builder)
        {
            // Get methods on the builder where the method name is 'Build'
            var buildMethods = builder.GetMethods().Where(m => m.Name == "Build");
            foreach (var method in buildMethods)
            {
                string methodSignature = $"{method?.DeclaringType?.Namespace}.{method?.DeclaringType?.Name}.Build(" +
                    string.Join(", ", method.GetParameters().Select(p => p.ParameterType));

                // Write a line to the output for each parameter where..
                foreach (var parameter in method.GetParameters().Where(p =>
                    // .. the parameter does not have the 'CodeConfigOnly' attribute
                    p.GetCustomAttribute(typeof(CodeConfigOnlyAttribute)) == null))
                {
                    // Get default value
                    var defaultValueAttr = parameter.GetCustomAttribute(typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                    var defaultValue = defaultValueAttr?.DefaultValue;
                    if (defaultValue == null)
                    {
                        defaultValue = "No default value";
                    }

                    // Write a comment about where to find out more about what this setting does.
                    writer.WriteCommentValue($"See {methodSignature}");

                    // Write the json property
                    WriteParameterLine(writer, parameter?.Name ?? "NULL", defaultValue);
                }
            }
        }

        /// <summary>
        /// Write a line containing the parameter and the default value.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="parameterName"></param>
        /// <param name="defaultValue"></param>
        private static void WriteParameterLine(Utf8JsonWriter writer, string parameterName, object defaultValue)
        {
            if (defaultValue == null)
            {
                writer.WriteString(parameterName, "No default");
            }
            else if (defaultValue.GetType() == typeof(string))
            {
                writer.WriteString(parameterName, (string)defaultValue);
            }
            else if (defaultValue.GetType() == typeof(bool))
            {
                writer.WriteBoolean(parameterName, (bool)defaultValue);
            }
            else if (defaultValue.GetType() == typeof(int))
            {
                writer.WriteNumber(parameterName, (int)defaultValue);
            }
            else if (defaultValue.GetType() == typeof(float))
            {
                writer.WriteNumber(parameterName, (float)defaultValue);
            }
            else
            {
                writer.WriteString(parameterName, $"Unexpected type - {defaultValue.GetType().Name}");
            }
        }
    }
}
