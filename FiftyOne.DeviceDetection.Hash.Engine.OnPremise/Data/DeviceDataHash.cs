/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers;
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System.Threading;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    /// <summary>
    /// Device data class
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/data-model.md">Specification</see>
    /// </summary>
    internal class DeviceDataHash : DeviceDataBaseOnPremise<IResultsSwigWrapper>, IDeviceDataHash, IDisposable
    {
        private bool disposedValue;
        private static ThreadLocal<ResultsHashSerializer> resultsHashSerializer = new ThreadLocal<ResultsHashSerializer>(() => (
            new ResultsHashSerializer()
        ));
        #region Constructor

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
        internal DeviceDataHash(
            ILogger<AspectDataBase> logger,
            IPipeline pipeline,
            DeviceDetectionHashEngine engine,
            IMissingPropertyService missingPropertyService)
            : base(logger, pipeline, engine, missingPropertyService)
        {
        }

        #endregion

        #region Internal Methods
        internal void SetResults(IResultsSwigWrapper results)
        {
            CheckState();
            Results.AddResult(results);
        }
        #endregion

        #region Private Methods

        private IAspectPropertyValue<int> GetDrift()
        {
            return new AspectPropertyValue<int>(
                Results.ResultsList.Max(r => r.getDrift()));
        }

        private IAspectPropertyValue<int> GetDifference()
        {
            return new AspectPropertyValue<int>(
                Results.ResultsList.Sum(r => r.getDifference()));
        }

        private IAspectPropertyValue<int> GetMatchedNodes()
        {
            return new AspectPropertyValue<int>(
                Results.ResultsList.Sum(r => r.getMatchedNodes()));
        }

        private IAspectPropertyValue<string> GetMethod()
        {
            return new AspectPropertyValue<string>(
                ((MatchMethods)Results.ResultsList
                    .Max(r => r.getMethod())).ToString());
        }

        private IAspectPropertyValue<int> GetIterations()
        {
            return new AspectPropertyValue<int>(
                Results.ResultsList.Sum(r => r.getIterations()));
        }

        /// <summary>
        /// Get a single native results instance. If there is only one available,
        /// that is what is returned. If there are more, then the first one which
        /// contains values for the requested property is returned.
        ///</summary>
        ///<param name="propertyName">used to select results from list</param>
        ///<returns>single native results instance</returns>
        private IResultsSwigWrapper GetSingleResults(string propertyName)
        {
            CheckState();
            return Results.ResultsList.Count == 1 ?
                Results.ResultsList[0] : GetResultsContainingProperty(propertyName);
        }

        private IResultsSwigWrapper GetResultsContainingProperty(string propertyName)
        {
            foreach (var results in Results.ResultsList)
            {
                if (results.containsProperty(propertyName))
                {
                    return results;
                }
            }
            return null;
        }

        private IAspectPropertyValue<string> GetDeviceId()
        {
            if (Results.ResultsList.Count == 1)
            {
                // Only one Engine has added results, so return the device
                // id from those results.
                return new AspectPropertyValue<string>(
                    Results.ResultsList[0].getDeviceId());
            }
            else
            {
                // Multiple Engines have added results, so construct a device
                // id from the results.
                var result = new List<string>();
                var deviceIds = new List<IList<string>>();
                foreach (var results in Results.ResultsList)
                {
                    deviceIds.Add(results.getDeviceId().Split('-'));
                }
                for (var i = 0; i < deviceIds.Max(d => d.Count); i++)
                {
                    var profileId = "0";
                    foreach (var deviceId in deviceIds)
                    {
                        if (deviceId.Count > i &&
                            deviceId[i].Equals("0", StringComparison.Ordinal) == false)
                        {
                            profileId = deviceId[i];
                            break;
                        }
                    }
                    result.Add(profileId);
                }
                return new AspectPropertyValue<string>(
                    string.Join("-", result));
            }
        }

        private IAspectPropertyValue<IReadOnlyList<string>> GetUserAgents()
        {
            var list = new List<string>();
            foreach (var results in Results.ResultsList)
            {
                for (uint i = 0; i < results.getUserAgents(); i++)
                {
                    var userAgent = results.getUserAgent(i);
                    if (list.Contains(userAgent) == false)
                    {
                        list.Add(userAgent);
                    }
                }
            }
            return new AspectPropertyValue<IReadOnlyList<string>>(list);
        }

        #endregion

        #region Protected Methods

        protected override bool TryGetValue<T>(string key, out T value)
        {
            CheckState();
            var result = base.TryGetValue(key, out value);
            if (result == false &&
                Results.HasResults())
            {
                object obj = null;
                if (key.Equals("DeviceId", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetDeviceId();
                    result = true;
                }
                else if (key.Equals("Method", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetMethod();
                    result = true;
                }

                else if (key.Equals("Difference", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetDifference();
                    result = true;
                }
                else if (key.Equals("Drift", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetDrift();
                    result = true;
                }
                else if (key.Equals("MatchedNodes", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetMatchedNodes();
                    result = true;
                }
                else if (key.Equals("Iterations", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetIterations();
                    result = true;
                }
                else if (key.Equals("UserAgents", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetUserAgents();
                    result = true;
                }
                if (result == true)
                {
                    try
                    {
                        value = (T)obj;
                    }
                    catch (InvalidCastException)
                    {
                        throw new Exception(
                            $"Expected property '{key}' to be of " +
                            $"type '{typeof(T).Name}' but it is " +
                            $"'{obj.GetType().Name}'");
                    }
                }
            }
            return result;
        }

        protected override bool PropertyIsAvailable(string propertyName)
        {
            CheckState();
            return Results.ResultsList
                .Any(r => r.containsProperty(propertyName));
        }

        protected override IAspectPropertyValue<bool> GetValueAsBool(string propertyName)
        {
            var result = new AspectPropertyValue<bool>();
            var results = GetSingleResults(propertyName);

            if (results != null)
            {
                using (var value = results.getValueAsBool(propertyName))
                {
                    if (value.hasValue())
                    {
                        result.Value = value.getValue();
                    }
                    else
                    {
                        result.NoValueMessage = value.getNoValueMessage();
                    }
                }
            }
            return result;
        }

        protected override IAspectPropertyValue<double> GetValueAsDouble(string propertyName)
        {
            var result = new AspectPropertyValue<double>();
            var results = GetSingleResults(propertyName);

            if (results != null)
            {
                using (var value = results.getValueAsDouble(propertyName))
                {
                    if (value.hasValue())
                    {
                        result.Value = value.getValue();
                    }
                    else
                    {
                        result.NoValueMessage = value.getNoValueMessage();
                    }
                }
            }
            return result;
        }

        protected override IAspectPropertyValue<int> GetValueAsInteger(string propertyName)
        {
            var result = new AspectPropertyValue<int>();
            var results = GetSingleResults(propertyName);

            if (results != null)
            {
                using (var value = results.getValueAsInteger(propertyName))
                {
                    if (value.hasValue())
                    {
                        result.Value = value.getValue();
                    }
                    else
                    {
                        result.NoValueMessage = value.getNoValueMessage();
                    }
                }
            }
            return result;
        }

        public override IAspectPropertyValue<IReadOnlyList<string>> GetValues(string propertyName)
        {
            var result = new AspectPropertyValue<IReadOnlyList<string>>();
            var results = GetSingleResults(propertyName);

            if (results != null)
            {
                using (var value = results.getValues(propertyName))
                {
                    if (value.hasValue())
                    {
                        using (var vector = value.getValue()) 
                        {
                            result.Value = vector.ToList();
                        }
                    }
                    else
                    {
                        result.NoValueMessage = value.getNoValueMessage();
                    }
                }
            }
            return result;
        }

        public string GetAllValuesJson()
        {
            if (Results.HasResults())
            {
                var results =  Results.ResultsList[0] as ResultsSwigWrapper;
                if (results != null)
                {
                    return resultsHashSerializer.Value.allValuesJson(results.Object);
                }
            }
            return null;
            
        }

        protected override IAspectPropertyValue<string> GetValueAsString(string propertyName)
        {
            var result = new AspectPropertyValue<string>();
            var results = GetSingleResults(propertyName);

            if (results != null)
            {
                using (var value = results.getValueAsString(propertyName))
                {
                    if (value.hasValue())
                    {
                        result.Value = value.getValue();
                    }
                    else
                    {
                        result.NoValueMessage = value.getNoValueMessage();
                    }
                }
            }
            return result;
        }

        protected override IAspectPropertyValue<JavaScript> GetValueAsJavaScript(string propertyName)
        {
            var result = new AspectPropertyValue<JavaScript>();
            var results = GetSingleResults(propertyName);

            if (results != null)
            {
                using (var value = results.getValueAsString(propertyName))
                {
                    if (value.hasValue())
                    {
                        result.Value = new JavaScript(value.getValue());
                    }
                    else
                    {
                        result.NoValueMessage = value.getNoValueMessage();
                    }
                }
            }
            return result;
        }

        private void CheckState()
        {
            if (disposedValue == true)
            {
                throw new InvalidOperationException(Messages.DeviceDataHashDisposed);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                foreach (var result in Results.ResultsList)
                {
                    try
                    {
                        result.Dispose();
                    }
                    catch (ObjectDisposedException e)
                    {
                        Logger.LogError(Messages.DeviceDataHashFailedToDispose, e);
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
