/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2023 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;

namespace FiftyOne.DeviceDetection.Shared.FlowElements
{

    /// <summary>
    /// Fluent builder that is used to create an on-premise device detection 
    /// aspect engine.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/pipeline-elements/device-detection-on-premise.md">Specification</see>
    /// </summary>
    /// <typeparam name="TBuilder">
    /// The type of engine builder for fluent methods to return.
    /// </typeparam>
    /// <typeparam name="TEngine">
    /// The type of engine that will be built. 
    /// </typeparam>
    public abstract class OnPremiseDeviceDetectionEngineBuilderBase<TBuilder, TEngine>
        : FiftyOneOnPremiseAspectEngineBuilderBase<TBuilder, TEngine>
        where TBuilder : OnPremiseDeviceDetectionEngineBuilderBase<TBuilder, TEngine>
        where TEngine : IFiftyOneAspectEngine
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataUpdateService">
        /// The <see cref="IDataUpdateService"/> to use when registering
        /// data files to check for for automatic updates.
        /// </param>
        public OnPremiseDeviceDetectionEngineBuilderBase(
            IDataUpdateService dataUpdateService)
            : base(dataUpdateService)
        {
        }

        /// <summary>
        /// Set the maximum difference to allow when processing HTTP headers.
        /// The difference is the difference in hash value between the 
        /// hash that was found, and the hash that is being searched for.
        /// By default this is 0.
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
        /// <returns>This builder</returns>
        public abstract TBuilder SetAllowUnmatched(bool allow);

        /// <summary>
        /// Set the expected number of concurrent operations using the engine.
        /// This sets the concurrency of the internal caches to mitigate
        /// excessive contention.
        /// </summary>
        /// <param name="concurrency">Expected concurrent accesses</param>
        /// <returns>This builder</returns>
        public abstract TBuilder SetConcurrency(ushort concurrency);

        /// <summary>
        /// Specify if the 'performance' evaluation graph should be used 
        /// or not.
        /// The performance graph is faster than predictive but can
        /// be less accurate.
        /// Note that the performance graph will always be evaluated first 
        /// if it is enableds so if you have both performance and predictive 
        /// enabled, you will often be getting results from just the 
        /// performance graph.
        /// In that situation, predictive will only be used if a match cannot
        /// be found using the performance graph.
        /// </summary>
        /// <param name="use">
        /// True to use the performance graph, false to ignore it.
        /// </param>
        /// <returns>
        /// This builder.
        /// </returns>
        public abstract TBuilder SetUsePerformanceGraph(bool use);

        /// <summary>
        /// Specify if the 'predictive' evaluation graph should be used 
        /// or not.
        /// The predictive graph is more accurate than performance
        /// but is also slower.
        /// Note that the performance graph will always be evaluated first 
        /// if it is enabled, so if you have both performance and predictive 
        /// enabled, you will often be getting results from just the 
        /// performance graph.
        /// In that situation, predictive will only be used if a match cannot
        /// be found using the performance graph.
        /// </summary>
        /// <param name="use">
        /// True to use the performance graph, false to ignore it.
        /// </param>
        /// <returns>
        /// This builder.
        /// </returns>
        public abstract TBuilder SetUsePredictiveGraph(bool use);
    }
}
