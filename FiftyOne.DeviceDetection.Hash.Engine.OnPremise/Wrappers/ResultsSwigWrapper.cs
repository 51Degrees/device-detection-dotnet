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
using System.Runtime.InteropServices;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class ResultsSwigWrapper : IResultsSwigWrapper
    {
        // Flattened, "by value" scalar accessors (issue #524, fix #4). These call
        // the hand-written native exports in DeviceDetectionFastValues.cpp, which
        // return the value and its has-value flag in a single P/Invoke with no
        // Value<T> (BoolValueSwig etc.) heap object. Used by the fast paths in
        // DeviceDataHash; see that class for the slow-path fallback that recovers
        // the no-value message.
        private const string NativeLib =
            "FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Native.dll";

        [DllImport(NativeLib, EntryPoint = "fiftyone_hash_get_bool", CharSet = CharSet.Ansi)]
        private static extern int fiftyone_hash_get_bool(IntPtr results, string name, out int hasValue);

        [DllImport(NativeLib, EntryPoint = "fiftyone_hash_get_int", CharSet = CharSet.Ansi)]
        private static extern int fiftyone_hash_get_int(IntPtr results, string name, out int hasValue);

        [DllImport(NativeLib, EntryPoint = "fiftyone_hash_get_double", CharSet = CharSet.Ansi)]
        private static extern double fiftyone_hash_get_double(IntPtr results, string name, out int hasValue);

        [DllImport(NativeLib, EntryPoint = "fiftyone_hash_get_string", CharSet = CharSet.Ansi)]
        private static extern int fiftyone_hash_get_string(IntPtr results, string name, byte[] buffer, int bufferLength, out int valueLength);

        // Reusable per-thread buffer for the string fast path, sized to cover
        // effectively all device-detection property values; longer values fall
        // back to the slow SWIG path.
        [ThreadStatic]
        private static byte[] _stringBuffer;

        public ResultsHashSwig Object { get; }

        public ResultsSwigWrapper(ResultsHashSwig instance)
        {
            Object = instance;
        }

        public bool TryGetBoolFast(string propertyName, out bool value)
        {
            int hasValue = 0;
            int boolAsInt = fiftyone_hash_get_bool(ResultsHashSwig.getCPtr(Object).Handle, propertyName, out hasValue);
            GC.KeepAlive(Object);
            value = boolAsInt != 0;
            return hasValue != 0;
        }

        public bool TryGetIntFast(string propertyName, out int value)
        {
            int hasValue = 0;
            value = fiftyone_hash_get_int(ResultsHashSwig.getCPtr(Object).Handle, propertyName, out hasValue);
            GC.KeepAlive(Object);
            return hasValue != 0;
        }

        public bool TryGetDoubleFast(string propertyName, out double value)
        {
            int hasValue = 0;
            value = fiftyone_hash_get_double(ResultsHashSwig.getCPtr(Object).Handle, propertyName, out hasValue);
            GC.KeepAlive(Object);
            return hasValue != 0;
        }

        public bool TryGetStringFast(string propertyName, out string value)
        {
            var buffer = _stringBuffer ?? (_stringBuffer = new byte[512]);
            int valueLength = 0;
            int hasValue = fiftyone_hash_get_string(
                ResultsHashSwig.getCPtr(Object).Handle, propertyName, buffer, buffer.Length, out valueLength);
            GC.KeepAlive(Object);
            // hasValue == 0 -> no value (caller falls back for the no-value message).
            // valueLength >= buffer.Length -> value too long to fit; fall back so
            // the slow path returns the full value.
            if (hasValue == 0 || valueLength >= buffer.Length)
            {
                value = null;
                return false;
            }
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
