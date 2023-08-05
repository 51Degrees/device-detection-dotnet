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

using FiftyOne.Pipeline.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FiftyOne.DeviceDetection.TestHelpers.Data
{
    public class ValueTests
    {
        public static void DeviceId(IWrapper wrapper)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.user-agent", Constants.MobileUserAgent)
                    .Process();
                var elementData = data.Get(wrapper.GetEngine().ElementDataKey);
                IDeviceData device = (IDeviceData)elementData;
                Assert.IsNotNull(device.DeviceId,
                    "The device id should not be null.");
                Assert.IsTrue(string.IsNullOrEmpty(device.DeviceId.Value) == false,
                    "The device id should not be empty.");
            }
        }

        public static void MatchedUserAgents(IWrapper wrapper)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.user-agent", Constants.MobileUserAgent)
                        .Process();
                var elementData = data.Get(wrapper.GetEngine().ElementDataKey);
                IDeviceData device = (IDeviceData)elementData;
                Assert.AreEqual(1, device.UserAgents.Value.Count);
                foreach (var matchedUa in device.UserAgents.Value)
                {
                    foreach (var substring in matchedUa.Split('_', '{', '}'))
                    {
                        Assert.IsTrue(Constants.MobileUserAgent.Contains(substring),
                            $"The matched substring '{substring}' does not " +
                            $"exist in the original User-Agent.");
                        var index = matchedUa.IndexOf(substring);
                        var original = Constants.MobileUserAgent
                            .Substring(index, substring.Length);
                        Assert.AreEqual(substring, original,
                            $"Expected to find substring '{original}' at character " +
                            $"position {index} but the substring found was " +
                            $"{substring}.");
                    }
                }
            }
        }


        public static void ValueTypes(IWrapper wrapper)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.user-agent", Constants.MobileUserAgent)
                        .Process();
                var elementData = data.Get(wrapper.GetEngine().ElementDataKey);
                foreach (var property in wrapper.GetEngine().Properties
                    .Where(p => p.Available))
                {
                    Type expectedType = property.Type;
                    var value = elementData[property.Name];
                    Assert.IsNotNull(value);

                    if (value != null)
                    {
                        Assert.IsTrue(expectedType.IsAssignableFrom(
                            value.GetType().GenericTypeArguments[0]),
                            $"Value of '{property.Name}' was of type " +
                            $"{value.GetType()} but should have been " +
                            $"{expectedType}.");
                    }
                }
            }
        }

        public static void AvailableProperties(IWrapper wrapper)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.user-agent", Constants.MobileUserAgent)
                        .Process();
                var elementData = data.Get(wrapper.GetEngine().ElementDataKey);
                foreach (var property in wrapper.GetEngine().Properties)
                {
                    var dict = elementData.AsDictionary();

                    Assert.AreEqual(property.Available, dict.ContainsKey(property.Name),
                        $"Property '{property.Name}' " +
                        $"{(property.Available ? "should" : "should not")} be in the results.");
                }
            }
        }

        public static void TypedGetters(IWrapper wrapper)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.user-agent", Constants.MobileUserAgent)
                        .Process();
                var elementData = data.Get(wrapper.GetEngine().ElementDataKey);
                var missingGetters = new List<string>();
                foreach (var property in wrapper.GetEngine().Properties
                    .Where(p => Constants.ExcludedProperties.Contains(p.Name) == false))
                {
                    var cleanPropertyName = property.Name
                        .Replace("/", "")
                        .Replace("-", "");
                    var classProperty = elementData.GetType().GetProperties()
                        .Where(p => p.Name == cleanPropertyName)
                        .FirstOrDefault();
                    if (classProperty != null)
                    {
                        var accessor = classProperty.GetAccessors().FirstOrDefault();
                        if (property.Available == true)
                        {
                            var value = accessor.Invoke(elementData, null);
                            Assert.IsNotNull(value,
                                $"The typed getter for '{property.Name}' should " +
                                $"not have returned a null value.");
                        }
                        else
                        {
                            try
                            {
                                var value = accessor.Invoke(elementData, null);
                                Assert.Fail(
                                    $"The property getter for '{property.Name}' " +
                                    $"should have thrown a " +
                                    $"PropertyMissingException.");
                            }
                            catch (TargetInvocationException e)
                            {
                                Assert.IsInstanceOfType(
                                    e.InnerException,
                                    typeof(PropertyMissingException),
                                    $"The property getter for '{property.Name}' " +
                                    $"should have thrown a " +
                                    $"PropertyMissingException, but the exception " +
                                    $"was of type '{e.InnerException.GetType()}'.");
                            }
                        }
                    }
                    else
                    {
                        missingGetters.Add(property.Name);
                    }
                }
                if (missingGetters.Count > 0)
                {
                    if (missingGetters.Count == 1)
                    {
                        Assert.Inconclusive($"The property '{missingGetters[0]}' " +
                        $"is missing a getter in the DeviceData class. This is not " +
                        $"a serious issue, and the property can still be used " +
                        $"through the AsDictionary method, but it is an indication " +
                        $"that the API should be updated in order to enable the " +
                        $"the strongly typed getter for this property.");
                    }
                    else
                    {
                        Assert.Inconclusive($"The properties " +
                        $"{string.Join(", ", missingGetters.Select(p => "'" + p + "'"))} " +
                        $"are missing getters in the DeviceData class. This is not " +
                        $"a serious issue, and the properties can still be used " +
                        $"through the AsDictionary method, but it is an indication " +
                        $"that the API should be updated in order to enable the " +
                        $"the strongly typed getter for these properties.");
                    }
                }
            }
        }
    }
}
