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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FiftyOne.DeviceDetection.TestHelpers
{
    public class UserAgentGenerator
    {
        private readonly string[] _userAgents;

        private Random _random = new Random();

        public UserAgentGenerator(FileInfo userAgentFile)
        {
            _userAgents = File.ReadAllLines(userAgentFile.FullName);
        }

        /// <summary>
        /// Returns a random user agent which may also have been randomised.
        /// </summary>
        /// <param name="randomness"></param>
        /// <returns></returns>
        public string GetRandomUserAgent(int randomness)
        {
            var value = _userAgents[_random.Next(_userAgents.Length)];
            if (randomness > 0)
            {
                var bytes = ASCIIEncoding.ASCII.GetBytes(value);
                for (int i = 0; i < randomness; i++ )
                {
                    var indexA = _random.Next(value.Length);
                    var indexB = _random.Next(value.Length);
                    byte temp = bytes[indexA];
                    bytes[indexA] = bytes[indexB];
                    bytes[indexB] = temp;
                }
                value = ASCIIEncoding.ASCII.GetString(bytes);
            }
            return value;
        }

        public IEnumerable<string> GetEnumerable(int count, int randomness)
        {
            for(int i = 0; i < count; i++)
            {
                yield return GetRandomUserAgent(randomness);
            }
        }

        /// <summary>
        /// Returns an enumerable of User-Agent strings which match the regex. The
        /// results can not return more than the count specified.
        /// </summary>
        /// <param name="count">Number of User-Agents to return.</param>
        /// <param name="pattern">Regular expression for the user agents.</param>
        /// <returns></returns>
        public IEnumerable<string> GetEnumerable(int count, string pattern)
        {
            var counter = 0;
            var regex = new Regex(pattern, RegexOptions.Compiled);
            while (counter < count)
            {
                var iterator = _userAgents.Select(i => i).GetEnumerator();
                while (counter < count && iterator.MoveNext())
                {
                    if (regex.IsMatch(iterator.Current))
                    {
                        yield return iterator.Current;
                        counter++;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerable of User-Agent strings. The results can not
        /// return more than the count specified.
        /// </summary>
        /// <param name="count">Number of User-Agents to return.</param>
        /// <returns></returns>
        public IEnumerable<string> GetEnumerable(int count)
        {
            return GetEnumerable(count, "(.*?)");
        }

        public IEnumerable<string> GetRandomUserAgents(int count)
        {
            return GetEnumerable(count);
        }

        public IEnumerable<string> GetUniqueUserAgents(int count)
        {
            return _userAgents.OrderBy(i => _random.Next()).Take(count);
        }

        public IEnumerable<string> GetBadUserAgents(int count)
        {
            return GetEnumerable(count, 10);
        }
    }
}