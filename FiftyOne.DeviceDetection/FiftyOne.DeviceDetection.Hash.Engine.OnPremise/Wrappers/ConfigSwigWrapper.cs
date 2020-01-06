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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class ConfigSwigWrapper : IConfigSwigWrapper
    {
        private ConfigHashSwig _object;

        public ConfigHashSwig Object { get => _object; }

        public ConfigSwigWrapper(ConfigHashSwig instance)
        {
            _object = instance;
        }

        public void setDifference(int difference)
        {
            _object.setDifference(difference);
        }

        public void setDrift(int drift)
        {
            _object.setDrift(drift);
        }

        public void setReuseTempFile(bool reuse)
        {
            _object.setReuseTempFile(reuse);
        }

        public void setUpdateMatchedUserAgent(bool update)
        {
            _object.setUpdateMatchedUserAgent(update);
        }

        public void setAllowUnmatched(bool allow)
        {
            _object.setAllowUnmatched(allow);
        }

        public void setLowMemory()
        {
            _object.setLowMemory();
        }

        public void setBalanced()
        {
            _object.setBalanced();
        }

        public void setBalancedTemp()
        {
            _object.setBalancedTemp();
        }

        public void setHighPerformance()
        {
            _object.setHighPerformance();
        }
        public void setMaxPerformance()
        {
            _object.setMaxPerformance();
        }

        public void setConcurrency(ushort concurrency)
        {
            _object.setConcurrency(concurrency);
        }

        public void setUseUpperPrefixHeaders(bool upperPrefix)
        {
            _object.setUseUpperPrefixHeaders(upperPrefix);
        }

        public void setTempDirectories(VectorStringSwig dirs)
        {
            _object.setTempDirectories(dirs);
        }

        public void setUseTempFile(bool useTemp)
        {
            _object.setUseTempFile(useTemp);
        }
    }
}
