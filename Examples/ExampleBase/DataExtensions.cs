using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Examples
{
    public static class DataExtensions
    {
        /// <summary>
        /// Execute the specified function on the supplied <see cref="IElementData"/> instance.
        /// If a <see cref="PropertyMissingException"/> occurs then the resulting string will
        /// contain 'Unknown' + the message from the exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static string TryGetValue<T>(this T data, Func<T, string> function)
            where T : IElementData
        {
            string result;
            try
            {
                result = function(data);
            }
            catch (PropertyMissingException pex)
            {
                result = $"Unknown ({pex.Message})";
            }
            return result;
        }

        /// <summary>
        /// Get a human-readable version of the specified <see cref="IAspectPropertyValue"/>.
        /// If no value has be set, the result will be 'Unknown' + the 
        /// <see cref="IAspectPropertyValue.NoValueMessage"/>.
        /// </summary>
        /// <param name="apv"></param>
        /// <returns></returns>
        public static string GetHumanReadable(this IAspectPropertyValue<string> apv)
        {
            return apv.HasValue ? apv.Value : $"Unknown ({apv.NoValueMessage})";
        }
        public static string GetHumanReadable(this IAspectPropertyValue<IReadOnlyList<string>> apv)
        {
            return apv.HasValue ? string.Join(", ", apv.Value) : $"Unknown ({apv.NoValueMessage})";
        }
        public static string GetHumanReadable(this IAspectPropertyValue<int> apv)
        {
            return apv.HasValue ? apv.Value.ToString() : $"Unknown ({apv.NoValueMessage})";
        }
    }
}
