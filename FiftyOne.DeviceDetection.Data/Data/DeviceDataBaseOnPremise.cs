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

using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Shared.Data
{
    /// <summary>
    /// Base class used for all 51Degrees on-premise device data.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/data-model.md">Specification</see>
    /// </summary>
    public abstract class DeviceDataBaseOnPremise<TResult> : DeviceDataBase
        where TResult : IDisposable
    {
        /// <summary>
        /// This inner class manages the lifetime and access to the 
        /// swig result sets.
        /// </summary>
        protected class ResultsManager : IDisposable
        {
            private object _resultsListLock = new object();
            private List<TResult> _resultsList = new List<TResult>();

            private bool _disposed = false;
            private object _disposeLock = new object();

            /// <summary>
            /// Get the list of result objects.
            /// After this property accessed, the <see cref="AddResult(TResult)"/> 
            /// method will throw an error if it is called.
            /// </summary>
            public IReadOnlyList<TResult> ResultsList
            {
                get
                {
                    lock (_resultsListLock)
                    {
                        ResultsAccessed = true;
                        return _resultsList;
                    }
                }
            }

            /// <summary>
            /// Check if there are results objects available.
            /// </summary>
            /// <returns>
            /// True if there are results objects available.
            /// </returns>
            public bool HasResults()
            {
                return _resultsList.Count > 0;
            }

            /// <summary>
            /// Add the specified result object to the list of results.
            /// </summary>
            /// <param name="result">
            /// The result object to add to the list.
            /// </param>
            /// <exception cref="PipelineException">
            /// Thrown if the <see cref="ResultsList"/> property for 
            /// this instance has already been accessed.
            /// </exception>
            public void AddResult(TResult result)
            {
                lock (_resultsListLock)
                {
                    if (ResultsAccessed == true)
                    {
                        throw new PipelineException(
                            Messages.ExceptionCannotUpdateResults);
                    }

                    _resultsList.Add(result);
                }
            }

            /// <summary>
            /// True if the <see cref="ResultsList"/> property for 
            /// this instance has been accessed. False is not.
            /// </summary>
            public bool ResultsAccessed { get; private set; } = false;

            #region IDisposable Support
            /// <summary>
            /// Dispose
            /// </summary>
            /// <param name="disposing">
            /// False if called from the finalizer
            /// </param>
            protected virtual void Dispose(bool disposing)
            {
                if (_disposed == false)
                {
                    lock (_disposeLock)
                    {
                        if (_disposed == false)
                        {
                            _disposed = true;

                            if (disposing)
                            {
                                // Dispose managed objects.
                            }

                            lock (_resultsListLock)
                            {
                                // Cleanup all unmanaged resources.
                                foreach (var results in _resultsList)
                                {
                                    if (results != null)
                                    {
                                        results.Dispose();
                                    }
                                }
                                _resultsList = null;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Finalizer
            /// </summary>
            ~ResultsManager()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(false);
            }

            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }

        #region Private Properties

        private object _dataLock = new object();
        private object _getLock = new object();

        private bool _dictionaryPopulated = false;

        /// <summary>
        /// A <see cref="ResultsManager"/> instance, which can be used
        /// to add new results objects and access those that have been
        /// added so far.
        /// Once results are accessed, new ones cannot be added.
        /// </summary>
        protected ResultsManager Results { get; private set; } = new ResultsManager();

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">
        /// The logger for this instance to use.
        /// </param>
        /// <param name="pipeline">
        /// The Pipeline this data instance has been created by.
        /// </param>
        /// <param name="engine">
        /// The engine this data instance has been created by.
        /// </param>
        /// <param name="missingPropertyService">
        /// The missing property service to use when a requested property
        /// does not exist.
        /// </param>
        protected DeviceDataBaseOnPremise(
            ILogger<AspectDataBase> logger,
            IPipeline pipeline,
            IAspectEngine engine,
            IMissingPropertyService missingPropertyService)
            : base(logger, pipeline, engine, missingPropertyService)
        {
        }

        #endregion

        #region Abstract Methods
        /// <summary>
        /// Check if a specified property is available in the results.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to check.
        /// </param>
        /// <returns>
        /// True if the property is available. False if not.
        /// </returns>
        protected abstract bool PropertyIsAvailable(string propertyName);

        /// <summary>
        /// Get the values this instance has for the specified property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get values for.
        /// </param>
        /// <returns>
        /// A list of <see cref="string"/> values wrapped in a 
        /// <see cref="IAspectPropertyValue"/> instance.
        /// </returns>
        public abstract IAspectPropertyValue<IReadOnlyList<string>> GetValues(string propertyName);

        /// <summary>
        /// Get the string value this instance has for the specified property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get value for.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> wrapped in a 
        /// <see cref="IAspectPropertyValue"/> instance.
        /// </returns>
        protected abstract IAspectPropertyValue<string> GetValueAsString(string propertyName);

        /// <summary>
        /// Get the integer value this instance has for the specified property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get value for.
        /// </param>
        /// <returns>
        /// An <see cref="int"/> wrapped in a 
        /// <see cref="IAspectPropertyValue"/> instance.
        /// </returns>
        protected abstract IAspectPropertyValue<int> GetValueAsInteger(string propertyName);

        /// <summary>
        /// Get the boolean value this instance has for the specified property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get value for.
        /// </param>
        /// <returns>
        /// A <see cref="bool"/> wrapped in a 
        /// <see cref="IAspectPropertyValue"/> instance.
        /// </returns>
        protected abstract IAspectPropertyValue<bool> GetValueAsBool(string propertyName);

        /// <summary>
        /// Get the numeric double value this instance has for the specified 
        /// property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get value for.
        /// </param>
        /// <returns>
        /// A <see cref="double"/> wrapped in a 
        /// <see cref="IAspectPropertyValue"/> instance.
        /// </returns>
        protected abstract IAspectPropertyValue<double> GetValueAsDouble(string propertyName);

        /// <summary>
        /// Get the value this instance has for the specified property as
        /// a <see cref="JavaScript"/> object.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get value for.
        /// </param>
        /// <returns>
        /// A <see cref="JavaScript"/> value wrapped in a 
        /// <see cref="IAspectPropertyValue"/> instance.
        /// </returns>
        protected abstract IAspectPropertyValue<JavaScript> GetValueAsJavaScript(string propertyName);

        #endregion

        #region Overrides
        /// <summary>
        /// By default, the base dictionary will not be populated as 
        /// doing so is a fairly expensive operation.
        /// Instead, we override the AsDictionary method to populate
        /// the dictionary on-demand.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", 
            "CA1308:Normalize strings to uppercase", 
            Justification = "Pipeline specification is for keys to be lower-case.")]
        public override IReadOnlyDictionary<string, object> AsDictionary()
        {
            if (_dictionaryPopulated == false)
            {
                lock(_dataLock)
                {
                    if (_dictionaryPopulated == false)
                    {
                        var dict = new Dictionary<string, object>(
                            StringComparer.InvariantCultureIgnoreCase);
                        foreach(var property in
                            Pipeline
                            .ElementAvailableProperties[Engines[0].ElementDataKey]
                            .Values)
                        {
                            if (property.Type == typeof(string))
                            {
                                dict[property.Name.ToLowerInvariant()] =
                                    GetAs<AspectPropertyValue<string>>(property.Name);
                            }
                            else if (property.Type == typeof(double))
                            {
                                dict[property.Name.ToLowerInvariant()] =
                                    GetAs<AspectPropertyValue<double>>(property.Name);
                            }
                            else if (property.Type == typeof(int))
                            {
                                dict[property.Name.ToLowerInvariant()] =
                                    GetAs<AspectPropertyValue<int>>(property.Name);
                            }
                            else if (property.Type == typeof(bool))
                            {
                                dict[property.Name.ToLowerInvariant()] =
                                    GetAs<AspectPropertyValue<bool>>(property.Name);
                            }
                            else if (property.Type == typeof(IReadOnlyList<string>))
                            {
                                dict[property.Name.ToLowerInvariant()] =
                                    GetAs<AspectPropertyValue<IReadOnlyList<string>>>(property.Name);
                            }
                            else if (property.Type == typeof(JavaScript))
                            {
                                dict[property.Name.ToLowerInvariant()] =
                                    GetAs<AspectPropertyValue<JavaScript>>(property.Name);
                            }
                            else
                            {
                                throw new Exception($"Unknown property type in " +
                                    $"data file. Property {property.Name} has " +
                                    $"type {property.Type.Name}");
                            }
                        };
                        PopulateFrom(dict);
                        _dictionaryPopulated = true;
                    }
                }
            }
            // Now that the base dictionary has been populated, 
            // we can return it.
            return base.AsDictionary();
        }
        
        /// <summary>
        /// Get the <see cref="Type"/> for the specified property
        /// This is based on the meta-data that is supplied by the engine.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get the type of.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> instance for the requested property.
        /// </returns>
        protected Type GetPropertyType(string propertyName)
        {
            Type type = typeof(object);
            var properties = Pipeline
                .ElementAvailableProperties[Engines[0].ElementDataKey];
            if (properties != null)
            {
                var property = properties[propertyName];
                if (property != null)
                {
                    type = property.Type;
                }
            }
            return type;
        }

        private static ConcurrentDictionary<string, Type> _innerTypes = new ConcurrentDictionary<string, Type>();
        private Type GetInnerType<T>(string propertyName)
        {
            return _innerTypes.GetOrAdd(propertyName,
                (string key) =>
                {
                    Type type = typeof(T);
                    Type innerType;
                    if (type == typeof(object))
                    {
                        innerType = GetPropertyType(key);
                    }
                    else
                    {
                        innerType = type.GenericTypeArguments[0];
                    }
                    return innerType;
                });
        }

        /// <summary>
        /// Try to get the value for the specified key.
        /// This overrides the base implementation to get values using
        /// the abstract methods on this class rather than using the
        /// dictionary-based storage mechanism from the base-class.
        /// </summary>
        /// <typeparam name="T">
        /// The expected type of the resulting value.
        /// </typeparam>
        /// <param name="key">
        /// The key to use to retrieve the value.
        /// </param>
        /// <param name="value">
        /// Out parameter that will be populated with the value.
        /// </param>
        /// <returns>
        /// True if that value was retrieved successfully. False if not.
        /// </returns>
        /// <exception cref="PipelineException">
        /// Thrown if the value was not of the expected type.
        /// </exception>
        protected override bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);
            if (_dictionaryPopulated == true)
            {
                // If the complete set of values has been populated 
                // then we can use the base implementation to get 
                // the value from the dictionary.
                return base.TryGetValue(key, out value);
            }
            else if (Results.HasResults())
            {
                // If the complete set of values has not been populated 
                // then we don't want to retrieve values for all 
                // properties so just get the one we want.
                bool result = PropertyIsAvailable(key);
                if (result)
                {
                    Type innerType = GetInnerType<T>(key);
                    lock (_getLock)
                    {
                        object obj = null;
                        if (innerType == typeof(string))
                        {
                            obj = GetValueAsString(key);
                        }
                        else if (innerType == typeof(double))
                        {
                            obj = GetValueAsDouble(key);
                        }
                        else if (innerType == typeof(int))
                        {
                            obj = GetValueAsInteger(key);
                        }
                        else if (innerType == typeof(bool))
                        {
                            obj = GetValueAsBool(key);
                        }
                        else if (innerType == typeof(IReadOnlyList<string>))
                        {
                            obj = GetValues(key);
                        }
                        else if (innerType == typeof(JavaScript))
                        {
                            obj = GetValueAsJavaScript(key);
                        }
                        else if (innerType == typeof(object))
                        {
                            obj = GetValues(key);
                        }
                        
                        try
                        {
                            value = (T)obj;
                        }
                        catch (InvalidCastException)
                        {
                            throw new PipelineException(
                                $"Expected property '{key}' to be of " +
                                $"type '{typeof(T).Name}' but it is " +
                                $"'{obj.GetType().Name}'");
                        }
                    }
                }
                return result;
            }
            return false;
        }
        #endregion
    }
}
