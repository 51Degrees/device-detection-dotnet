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

using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Interop;
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Data
{
    internal class DeviceDataPattern : DeviceDataBaseOnPremise, IDeviceDataPattern
    {
        #region Fields

        private IList<ResultsPatternSwig> _resultsList = new List<ResultsPatternSwig>();

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new instance of the wrapper.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="pipeline"></param>
        /// <param name="engine"></param>
        /// <param name="missingPropertyService"></param>
        internal DeviceDataPattern(
            ILogger<AspectDataBase> logger,
            IPipeline pipeline,
            DeviceDetectionPatternEngine engine,
            IMissingPropertyService missingPropertyService)
            : base(logger, pipeline, engine, missingPropertyService)
        {
        }

        #endregion

        #region Public Properties

        public IAspectPropertyValue<string> Method
        {
            get
            {
                return GetAs<IAspectPropertyValue<string>>("Method");
            }
        }

        public IAspectPropertyValue<int> Rank
        {
            get
            {
                return GetAs<IAspectPropertyValue<int>>("Rank");
            }
        }

        public IAspectPropertyValue<int> Difference
        {
            get
            {
                return GetAs<IAspectPropertyValue<int>>("Difference");
            }
        }

        public IAspectPropertyValue<int> SignaturesCompared
        {
            get
            {
                return GetAs<IAspectPropertyValue<int>>("SignaturesCompared");
            }
        }

        #endregion

        #region Internal Methods
        internal void SetResults(ResultsPatternSwig results)
        {
            _resultsList.Add(results);
        }
        #endregion

        #region Private Methods

        private ResultsPatternSwig GetResultsContainingProperty(string propertyName)
        {
            foreach (var results in _resultsList)
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
            if (_resultsList.Count == 1)
            {
                // Only one Engine has added results, so return the device
                // id from those results.
                return new AspectPropertyValue<string>(_resultsList[0].getDeviceId());
            }
            else
            {
                // Multiple Engines have added results, so construct a device
                // id from the results.
                var result = new List<string>();
                var deviceIds = new List<IList<string>>();
                foreach (var results in _resultsList)
                {
                    deviceIds.Add(results.getDeviceId().Split('-'));
                }
                for (var i = 0; i < deviceIds.Max(d => d.Count); i++)
                {
                    var profileId = "0";
                    foreach (var deviceId in deviceIds)
                    {
                        if (deviceId.Count > i && deviceId[i].Equals("0") == false)
                        {
                            profileId = deviceId[i];
                            break;
                        }
                    }
                    result.Add(profileId);
                }
                return new AspectPropertyValue<string>(string.Join("-", result));
            }
        }

        private IAspectPropertyValue<string> GetMethod()
        {
            return new AspectPropertyValue<string>(
                ((MatchMethods)_resultsList.Max(r => r.getMethod())).ToString());
        }

        private IAspectPropertyValue<int> GetDifference()
        {
            return new AspectPropertyValue<int>(
                _resultsList.Sum(r => r.getDifference()));
        }

        private IAspectPropertyValue<int> GetRank()
        {
            return new AspectPropertyValue<int>(
                _resultsList.Min(r => r.getRank()));
        }

        private IAspectPropertyValue<int> GetSignaturesCompared()
        {
            return new AspectPropertyValue<int>(
                _resultsList.Sum(r => r.getSignaturesCompared()));
        }

        private IAspectPropertyValue<IReadOnlyList<string>> GetUserAgents()
        {
            var list = new List<string>();
            foreach (var results in _resultsList)
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
            var result = base.TryGetValue(key, out value);
            if (result == false)
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
                else if (key.Equals("Rank", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetRank();
                    result = true;
                }
                else if (key.Equals("Difference", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetDifference();
                    result = true;
                }
                else if (key.Equals("UserAgents", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetUserAgents();
                    result = true;
                }
                else if (key.Equals("SignaturesCompared", StringComparison.InvariantCultureIgnoreCase))
                {
                    obj = GetSignaturesCompared();
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
            return _resultsList.Any(r =>r.containsProperty(propertyName));
        }

        protected override IAspectPropertyValue<bool> GetValueAsBool(string propertyName)
        {
            var result = new AspectPropertyValue<bool>();
            var results = GetResultsContainingProperty(propertyName);
            if (results != null)
            {
                var value = results.getValueAsBool(propertyName);
                if (value.hasValue())
                {
                    result.Value = value.getValue();
                }
                else
                {
                    result.NoValueMessage = value.getNoValueMessage();
                }
            }
            return result;
        }

        protected override IAspectPropertyValue<double> GetValueAsDouble(string propertyName)
        {
            var result = new AspectPropertyValue<double>();
            var results = GetResultsContainingProperty(propertyName);
            if (results != null)
            {
                var value = results.getValueAsDouble(propertyName);
                if (value.hasValue())
                {
                    result.Value = value.getValue();
                }
                else
                {
                    result.NoValueMessage = value.getNoValueMessage();
                }
            }
            return result;
        }

        protected override IAspectPropertyValue<JavaScript> GetValueAsJavaScript(string propertyName)
        {
            var result = new AspectPropertyValue<JavaScript>();
            var results = GetResultsContainingProperty(propertyName);
            if (results != null)
            {
                var value = results.getValueAsString(propertyName);
                if (value.hasValue())
                {
                    result.Value = new JavaScript(value.getValue());
                }
                else
                {
                    result.NoValueMessage = value.getNoValueMessage();
                }
            }
            return result;
        }

        protected override IAspectPropertyValue<int> GetValueAsInteger(string propertyName)
        {
            var result = new AspectPropertyValue<int>();
            var results = GetResultsContainingProperty(propertyName);
            if (results != null)
            {
                var value = results.getValueAsInteger(propertyName);
                if (value.hasValue())
                {
                    result.Value = value.getValue();
                }
                else
                {
                    result.NoValueMessage = value.getNoValueMessage();
                }
            }
            return result;
        }
        
        protected override IAspectPropertyValue<string> GetValueAsString(string propertyName)
        {
            var result = new AspectPropertyValue<string>();
            var results = GetResultsContainingProperty(propertyName);

            if (results != null)
            {
                var value = results.getValueAsString(propertyName);
                if (value.hasValue())
                {
                    result.Value = value.getValue();
                }
                else
                {
                    result.NoValueMessage = value.getNoValueMessage();
                }
            }
            return result;
        }

        public override IAspectPropertyValue<IReadOnlyList<string>> GetValues(string propertyName)
        {
            var result = new AspectPropertyValue<IReadOnlyList<string>>();
            var results = GetResultsContainingProperty(propertyName);
            if (results != null)
            {
                var value = results.getValues(propertyName);
                if (value.hasValue())
                {
                    result.Value = value.getValue().ToList();
                }
                else
                {
                    result.NoValueMessage = value.getNoValueMessage();
                }
            }
            return result;
        }
        #endregion

        #region Finalizer
        private bool _disposed = false;
        private object _disposeLock = new object();

        ~DeviceDataPattern()
        {
            if (_disposed == false)
            {
                lock (_disposeLock)
                {
                    if (_disposed == false)
                    {
                        _disposed = true;

                        // Cleanup all unmanaged resources.
                        foreach (var results in _resultsList)
                        {
                            if (results != null)
                            {
                                results.Dispose();
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
