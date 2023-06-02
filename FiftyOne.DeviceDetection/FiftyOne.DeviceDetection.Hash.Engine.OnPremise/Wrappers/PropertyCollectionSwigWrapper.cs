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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System.Collections;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class PropertyCollectionSwigWrapper : IPropertyCollectionSwigWrapper
    {
        private PropertyMetaDataCollectionSwig _object;
        private DeviceDetectionHashEngine _engine;

        public PropertyCollectionSwigWrapper(
            PropertyMetaDataCollectionSwig instance,
            DeviceDetectionHashEngine engine)
        {
            _object = instance;
            _engine = engine;
        }

        public void Dispose()
        {
            _object.Dispose();
        }

        public IEnumerator<IFiftyOneAspectPropertyMetaData> GetEnumerator()
        {
            for (uint i = 0; i < _object.getSize(); i++)
            {
                yield return new PropertyMetaDataHash(_engine, _object.getByIndex(i));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
