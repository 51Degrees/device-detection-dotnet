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
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.Cloud.Data
{
    public class DeviceDataCloud : DeviceDataBase, IDeviceData
    {
        private Dictionary<string, string> _noValueReasons = null;
        public DeviceDataCloud(ILogger<AspectDataBase> logger,
            IFlowData flowData,
            IAspectEngine engine,
            IMissingPropertyService missingPropertyService)
            : base(logger, flowData, engine, missingPropertyService)
        {
        }

        public void SetNoValueReasons(Dictionary<string,string> noValueReasons)
        {
            _noValueReasons = noValueReasons
                .Where(r => r.Key.Split('.')[0] == Engines.FirstOrDefault().ElementDataKey)
                .ToDictionary(k => k.Key.Split('.')[1], v => v.Value);
        }

        public override IAspectPropertyValue<IReadOnlyList<string>> UserAgents => throw new NotImplementedException();

        public override IAspectPropertyValue<string> DeviceId => throw new NotImplementedException();

        /// <summary>
        /// Try to get the value from the base dictionary and convert it to the
        /// requested type. If requesting an AspectProperty then convert to the
        /// inner type and set the no value reason if there is no value and a 
        /// no value reason has been provided from the cloud request engine.
        /// </summary>
        /// <typeparam name="T">requested type</typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool TryGetValue<T>(string key, out T value)
        {
            if (typeof(IAspectPropertyValue).IsAssignableFrom(typeof(T)))
            {
                // Get the inner type of the AspectPropertyValue
                object obj;
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

                if (AsDictionary().TryGetValue(key, out obj))
                {
                    try
                    {
                        IAspectPropertyValue temp = null;
                        if (innerType == typeof(string))
                        {
                            temp = SetValue<string>(key, obj);
                        }
                        else if (innerType == typeof(double))
                        {
                            temp = SetValue<double>(key, obj);
                        }
                        else if (innerType == typeof(int))
                        {
                            temp = SetValue<int>(key, obj);
                        }
                        else if (innerType == typeof(bool))
                        {
                            temp = SetValue<bool>(key, obj);
                        }
                        else if (innerType == typeof(IReadOnlyList<string>))
                        {
                            temp = SetValue<IReadOnlyList<string>>(key, obj);
                        }
                        else if (innerType == typeof(JavaScript))
                        {
                            temp = SetValue<JavaScript>(key, obj);
                        }
                        else
                        {
                            throw new Exception($"Unknown property type in " +
                                $"data. Property {key} has " +
                                $"type {obj.GetType().Name}");
                        }

                        value = (T)temp;
                    }
                    catch (InvalidCastException)
                    {
                        throw new Exception(
                            $"Expected property '{key}' to be of " +
                            $"type '{typeof(T).Name}' but it is " +

                            $"'{obj.GetType().Name}'");
                    }
                    return true;
                }
                value = default(T);
                return false;
            }
            else
                return base.TryGetValue(key, out value);
        }

        /// <summary>
        /// Set the value based on the type or add a no value reason if there 
        /// is no value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private IAspectPropertyValue SetValue<T>(string key, object obj)
        {
            var temp = new AspectPropertyValue<T>();
            if (obj == null)
            {
                temp.NoValueMessage = _noValueReasons.Single(r => r.Key.Split('.')[1] == key).Value;
            }
            else
            {
                temp.Value = (T)obj;
            }
            return temp;
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
    }
}
