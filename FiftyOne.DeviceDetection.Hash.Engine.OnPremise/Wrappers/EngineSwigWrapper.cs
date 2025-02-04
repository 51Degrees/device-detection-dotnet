/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class EngineSwigWrapper : IEngineSwigWrapper
    {
        private EngineHashSwig _object;

        public EngineSwigWrapper(EngineHashSwig instance)
        {
            _object = instance;
        }

        public void Dispose()
        {
            _object.Dispose();
        }
        
        public ResultsHashSwig process(EvidenceDeviceDetectionSwig evidence)
        {
            return _object.process(evidence);
        }

        public void refreshData()
        {
            _object.refreshData();
        }

        public void refreshData(byte[] data, int dataSize)
        {
            _object.refreshData(data, dataSize);
        }

        public bool getAutomaticUpdatesEnabled()
        {
            return _object.getAutomaticUpdatesEnabled();
        }

        public string getDataFileTempPath()
        {
            return _object.getDataFileTempPath();
        }

        public VectorStringSwig getKeys()
        {
            return _object.getKeys();
        }

        public IMetaDataSwigWrapper getMetaData()
        {
            return new MetaDataSwigWrapper(_object.getMetaData());
        }

        public IDateSwigWrapper getPublishedTime()
        {
            return new DateSwigWrapper(_object.getPublishedTime());
        }

        public string getType()
        {
            return _object.getType();
        }

        public string getProduct()
        {
            return _object.getProduct();
        }

        public IDateSwigWrapper getUpdateAvailableTime()
        {
            return new DateSwigWrapper(_object.getUpdateAvailableTime());
        }
    }
}
