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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class MetaDataSwigWrapper : IMetaDataSwigWrapper
    {
        private MetaDataSwig _object;

        public MetaDataSwigWrapper(MetaDataSwig instance)
        {
            _object = instance;
        }

        public ComponentMetaDataSwig getComponentForProperty(
            PropertyMetaDataSwig property)
        {
            return _object.getComponentForProperty(property);
        }

        public PropertyMetaDataCollectionSwig getPropertiesForComponent(
            ComponentMetaDataSwig component)
        {
            return _object.getPropertiesForComponent(component);
        }

        public ComponentMetaDataSwig getComponentForProfile(
            ProfileMetaDataSwig profile)
        {
            return _object.getComponentForProfile(profile);
        }

        public ValueMetaDataCollectionSwig getValuesForProfile(
            ProfileMetaDataSwig profile)
        {
            return _object.getValuesForProfile(profile);
        }

        public ProfileMetaDataSwig getDefaultProfileForComponent(
            ComponentMetaDataSwig component)
        {
            return _object.getDefaultProfileForComponent(component);
        }

        public ValueMetaDataSwig getDefaultValueForProperty(
            PropertyMetaDataSwig property)
        {
            return _object.getDefaultValueForProperty(property);
        }

        public IValueCollectionSwigWrapper getValuesForProperty(
            PropertyMetaDataSwig property,
            DeviceDetectionHashEngine engine)
        {
            return new ValueCollectionSwigWrapper(_object.getValuesForProperty(property), engine);
        }

        public IComponentCollectionSwigWrapper getComponents(
            DeviceDetectionHashEngine engine)
        {
            return new ComponentCollectionSwigWrapper(_object.getComponents(), engine);
        }

        public IPropertyCollectionSwigWrapper getProperties(
            DeviceDetectionHashEngine engine)
        {
            return new PropertyCollectionSwigWrapper(_object.getProperties(), engine);
        }

        public IProfileCollectionSwigWrapper getProfiles(
            DeviceDetectionHashEngine engine)
        {
            return new ProfileCollectionSwigWrapper(_object.getProfiles(), engine);
        }

        public IValueCollectionSwigWrapper getValues(
            DeviceDetectionHashEngine engine)
        {
            return new ValueCollectionSwigWrapper(_object.getValues(), engine);
        }

        public PropertyMetaDataSwig getPropertyForValue(ValueMetaDataSwig value)
        {
            return _object.getPropertyForValue(value);
        }
    }
}
