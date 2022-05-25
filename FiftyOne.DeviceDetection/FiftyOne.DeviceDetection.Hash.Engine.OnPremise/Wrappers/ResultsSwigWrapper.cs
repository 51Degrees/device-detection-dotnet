/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class ResultsSwigWrapper : IResultsSwigWrapper
    {
        public ResultsHashSwig Object { get; }

        public ResultsSwigWrapper(ResultsHashSwig instance)
        {
            Object = instance;
        }
        public bool containsProperty(string propertyName)
        {
            return Object.containsProperty(propertyName);
        }

        public string getDeviceId()
        {
            return Object.getDeviceId();
        }

        public int getDifference()
        {
            return Object.getDifference();
        }

        public int getDrift()
        {
            return Object.getDrift();
        }

        public int getIterations()
        {
            return Object.getIterations();
        }

        public int getMatchedNodes()
        {
            return Object.getMatchedNodes();
        }

        public int getMethod()
        {
            return Object.getMethod();
        }

        public string getUserAgent(uint resultIndex)
        {
            return Object.getUserAgent(resultIndex);
        }

        public int getUserAgents()
        {
            return Object.getUserAgents();
        }

        public IValueSwigWrapper<bool> getValueAsBool(string propertyName)
        {
            return new BoolValueSwigWrapper(Object.getValueAsBool(propertyName));
        }

        public IValueSwigWrapper<double> getValueAsDouble(string propertyName)
        {
            return new DoubleValueSwigWrapper(Object.getValueAsDouble(propertyName));
        }

        public IValueSwigWrapper<int> getValueAsInteger(string propertyName)
        {
            return new IntegerValueSwigWrapper(Object.getValueAsInteger(propertyName));
        }

        public IValueSwigWrapper<string> getValueAsString(string propertyName)
        {
            return new StringValueSwigWrapper(Object.getValueAsString(propertyName));
        }

        public IValueSwigWrapper<VectorStringSwig> getValues(string propertyName)
        {
            return new VectorValueSwigWrapper(Object.getValues(propertyName));
        }

        public void Dispose()
        {
            Object.Dispose();
        }
    }
}
