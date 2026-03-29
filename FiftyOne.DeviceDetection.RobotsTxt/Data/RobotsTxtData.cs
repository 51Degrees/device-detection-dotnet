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
using Microsoft.Extensions.Logging;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Data
{
    /// <summary>
    /// </summary>
    public class RobotsTxtData : AspectDataBase, IRobotsTxtData
    {
        /// <summary>
        /// Constructs a new instance of <see cref="RobotsTxtData"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="pipeline">The pipeline this data belongs to.</param>
        /// <param name="engine">The engine that created this data.</param>
        public RobotsTxtData(
            ILogger<RobotsTxtData> logger,
            IPipeline pipeline,
            RobotsTxtEngine engine)
            : base(logger, pipeline, engine)
        {
        }

        /// <summary>
        /// See <see cref="RobotsTxt.Messages.PlainTextDescription"/>
        /// </summary>
        public IAspectPropertyValue<string> PlainText => 
            GetAs<IAspectPropertyValue<string>>(nameof(PlainText));

        /// <summary>
        /// See <see cref="RobotsTxt.Messages.AnnotatedTextDescription"/>
        /// </summary>
        public IAspectPropertyValue<string> AnnotatedText => 
            GetAs<IAspectPropertyValue<string>>(nameof(AnnotatedText));
    }
}
