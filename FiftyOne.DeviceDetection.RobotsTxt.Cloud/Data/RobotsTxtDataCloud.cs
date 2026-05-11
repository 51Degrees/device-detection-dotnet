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

using FiftyOne.DeviceDetection.RobotsTxt.Data;
using FiftyOne.DeviceDetection.RobotsTxt.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.RobotsTxt.Cloud.Data;

/// <summary>
/// A data class that makes working with robots.txt data from cloud
/// services easier for the user.
/// </summary>
public class RobotsTxtDataCloud : AspectDataBase, IRobotsTxtData
{
    /// <summary>
    /// Construct a new instance of the wrapper.
    /// </summary>
    /// <param name="logger">
    /// The logger instance to use.
    /// </param>
    /// <param name="pipeline">
    /// The Pipeline that created this data instance.
    /// </param>
    /// <param name="engine">
    /// The engine that create this data instance.
    /// </param>
    /// <param name="missingPropertyService">
    /// The <see cref="IMissingPropertyService"/> to use if a requested
    /// property does not exist.
    /// </param>
    public RobotsTxtDataCloud(ILogger<AspectDataBase> logger,
        IPipeline pipeline,
        IAspectEngine engine,
        IMissingPropertyService missingPropertyService)
        : base(logger, pipeline, engine, missingPropertyService)
    {
    }

    /// <summary>
    /// Dictionary of property value types, keyed on the string
    /// name of the type.
    /// </summary>
    protected static readonly IReadOnlyDictionary<string, Type> PropertyTypes =
        new Dictionary<string, Type>()
        {
            { "PlainText", typeof(IAspectPropertyValue<string>) },
            { "AnnotatedText", typeof(IAspectPropertyValue<string>) }
        };

    internal static bool TryGetPropertyType(string propertyName, out Type type)
    {
        if (PropertyTypes.ContainsKey(propertyName))
        {
            type = PropertyTypes[propertyName];
            return true;
        }
        else
        {
            type = null;
            return false;
        }
    }

    /// <summary>
    /// A simple version of the robots.txt lines with minimal comments..
    /// </summary>
    public IAspectPropertyValue<string> PlainText =>
        GetAs<IAspectPropertyValue<string>>(nameof(PlainText));

    /// <summary>
    /// A version of the robots.txt not intended for production use that 
    /// contains comments explaining how each crawler is being treated and 
    /// URLs to find out more information..
    /// </summary>
    public IAspectPropertyValue<string> AnnotatedText =>
        GetAs<IAspectPropertyValue<string>>(nameof(AnnotatedText));
}
