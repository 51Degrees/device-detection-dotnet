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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class ResultsSwigWrapper : IResultsSwigWrapper
    {
        // Flattened, "by value" scalar accessors (issue #524, fix #4). These call
        // the SWIG %extend tryGet* methods on ResultsHashSwig (see hash_csharp.i),
        // which return the value and its has-value flag in a single P/Invoke with
        // no Value<T> (BoolValueSwig etc.) heap object. Used by the fast paths in
        // DeviceDataHash; see that class for the slow-path fallback that recovers
        // the no-value message. A disposed proxy passes a null pointer, which the
        // native side reports as no-value so the slow path raises the usual
        // exception.

        // Reusable per-thread buffer for the string fast path. Sized (4 KB) to
        // cover even large JavaScript-property snippets so they hit the fast path
        // too. On a miss the value falls back to the slow SWIG path, which is
        // slightly slower than pre-PR here (the native call has already built the
        // full string to measure its length), so the buffer is kept generous.
        [ThreadStatic]
        private static byte[] _stringBuffer;

        private const int StringBufferSize = 4096;

        public ResultsHashSwig Object { get; }

        public ResultsSwigWrapper(ResultsHashSwig instance)
        {
            Object = instance;
        }

        public bool TryGetBoolFast(string propertyName, out bool value)
        {
            return Object.tryGetBool(propertyName, out value);
        }

        public bool TryGetIntFast(string propertyName, out int value)
        {
            return Object.tryGetInt(propertyName, out value);
        }

        public bool TryGetDoubleFast(string propertyName, out double value)
        {
            return Object.tryGetDouble(propertyName, out value);
        }

        public bool TryGetStringFast(string propertyName, out string value)
        {
            var buffer = _stringBuffer ?? (_stringBuffer = new byte[StringBufferSize]);
            bool hasValue = Object.tryGetString(
                propertyName, buffer, buffer.Length, out int valueLength);
            // hasValue false -> no value (caller falls back for the no-value message).
            // valueLength >= buffer.Length -> value too long to fit; fall back so
            // the slow path returns the full value.
            if (hasValue == false || valueLength >= buffer.Length)
            {
                value = null;
                return false;
            }
            // Decode as UTF-8, the encoding the data file stores values in. Note
            // the SWIG slow path marshals std::string as ANSI/LPStr (system code
            // page): for ASCII values - all current DD data - the two agree, and
            // for any non-ASCII byte UTF-8 here is the more correct result.
            value = Encoding.UTF8.GetString(buffer, 0, valueLength);
            return true;
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
