/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL) 
 * v.1.2 and is subject to its terms as set out below.
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

using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Shared.FlowElements
{
    public abstract class OnPremiseDeviceDetectionEngineBuilderBase<TBuilder, TEngine>
        : FiftyOneOnPremiseAspectEngineBuilderBase<TBuilder, TEngine>
        where TBuilder : OnPremiseDeviceDetectionEngineBuilderBase<TBuilder, TEngine>
        where TEngine : IFiftyOneAspectEngine
    {
        public OnPremiseDeviceDetectionEngineBuilderBase(
            IDataUpdateService dataUpdateService)
            : base(dataUpdateService)
        {
        }

        /// <summary>
        /// Set the maximum difference to allow when processing HTTP headers.
        /// The meaning of difference depends on the Device Detection API being
        /// used.
        /// For Pattern: The difference is a combination of the difference in
        ///              character position of matched substrings, and the
        ///              difference in ASCII value of each character of matched
        ///              substrings. By default this is 10.
        /// For Hash: The difference is the difference in hash value between
        ///           the hash that was found, and the hash that is being
        ///           searched for. By default this is 0.
        /// </summary>
        /// <param name="difference">Difference to allow</param>
        /// <returns>This builder</returns>
        public abstract TBuilder SetDifference(int difference);

        /// <summary>
        /// If set to false, a non-matching User-Agent will result in
        /// properties without set values. If set to true, a non-matching
        /// User-Agent will cause the 'default profiles' to be returned. This
        /// means that properties will always have values (i.e. no need to
        /// check .HasValue) but some may be inaccurate. By default, this is
        /// false.
        /// </summary>
        /// <param name="allow">
        /// True if results with no matched hash nodes or substrings should be
        /// considered valid
        /// </param>
        /// <returns>This builder
        public abstract TBuilder SetAllowUnmatched(bool allow);

        /// <summary>
        /// Set the expected number of concurrent operations using the engine.
        /// This sets the concurrency of the internal caches to mitigate
        /// excessive contention.
        /// </summary>
        /// <param name="concurrency">Expected concurrent accesses</param>
        /// <returns>This builder</returns>
        public abstract TBuilder SetConcurrency(ushort concurrency);
    }
}
