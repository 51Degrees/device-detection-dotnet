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

using FiftyOne.DeviceDetection.Shared;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FiftyOne.DeviceDetection.Shared.Data
{
    /// <summary>
    /// Base class used for all 51Degrees on-premise results classes.
    /// </summary>
    public abstract class DeviceDataBaseOnPremise : DeviceDataBase
    {
        #region Private Properties

        private object _dataLock = new object();
        private object _getLock = new object();

        private bool _dictionaryPopulated = false;

        #endregion

        #region Constructor

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

        protected abstract bool PropertyIsAvailable(string propertyName);

        public abstract IAspectPropertyValue<IReadOnlyList<string>> GetValues(string propertyName);

        protected abstract IAspectPropertyValue<string> GetValueAsString(string propertyName);

        protected abstract IAspectPropertyValue<int> GetValueAsInteger(string propertyName);

        protected abstract IAspectPropertyValue<bool> GetValueAsBool(string propertyName);

        protected abstract IAspectPropertyValue<double> GetValueAsDouble(string propertyName);

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
                                dict[property.Name.ToLower()] =
                                    GetAs<AspectPropertyValue<string>>(property.Name);
                            }
                            else if (property.Type == typeof(double))
                            {
                                dict[property.Name.ToLower()] =
                                    GetAs<AspectPropertyValue<double>>(property.Name);
                            }
                            else if (property.Type == typeof(int))
                            {
                                dict[property.Name.ToLower()] =
                                    GetAs<AspectPropertyValue<int>>(property.Name);
                            }
                            else if (property.Type == typeof(bool))
                            {
                                dict[property.Name.ToLower()] =
                                    GetAs<AspectPropertyValue<bool>>(property.Name);
                            }
                            else if (property.Type == typeof(IReadOnlyList<string>))
                            {
                                dict[property.Name.ToLower()] =
                                    GetAs<AspectPropertyValue<IReadOnlyList<string>>>(property.Name);
                            }
                            else if (property.Type == typeof(JavaScript))
                            {
                                dict[property.Name.ToLower()] =
                                    GetAs<AspectPropertyValue<JavaScript>>(property.Name);
                            }
                            else
                            {
                                throw new Exception($"Unknown property type in " +
                                    $"data file. Property {property.Name} has " +
                                    $"type {property.Type.Name}");
                            }
                        };
                        PopulateFromDictionary(dict);
                        _dictionaryPopulated = true;
                    }
                }
            }
            // Now that the base dictionary has been populated, 
            // we can return it.
            return base.AsDictionary();
        }
        
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
            else
            {
                // If the complete set of values has not been populated 
                // then we don't want to retrieve values for all 
                // properties so just get the one we want.
                bool result = PropertyIsAvailable(key);
                if (result)
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
                            throw new Exception(
                                $"Expected property '{key}' to be of " +
                                $"type '{typeof(T).Name}' but it is " +
                                $"'{obj.GetType().Name}'");
                        }
                    }
                }
                return result;
            }
        }

        #endregion
    }
}
