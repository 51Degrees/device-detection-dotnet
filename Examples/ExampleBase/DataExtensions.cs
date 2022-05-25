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
