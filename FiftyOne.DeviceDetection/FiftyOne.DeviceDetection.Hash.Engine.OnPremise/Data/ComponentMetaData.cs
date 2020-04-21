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

using System;
using System.Collections.Generic;
using System.Linq;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    public class ComponentMetaData : IComponentMetaData, IDisposable
    {
        private readonly DeviceDetectionHashEngine _engine;

        private readonly ComponentMetaDataSwig _source;

        internal ComponentMetaData(DeviceDetectionHashEngine engine, ComponentMetaDataSwig source)
        {
            _engine = engine;
            _source = source;
        }

        public IProfileMetaData DefaultProfile => new ProfileMetaData(
            _engine,
            _engine.MetaData.getDefaultProfileForComponent(_source));

        public IReadOnlyList<IFiftyOneAspectPropertyMetaData> Properties =>
            GetProperties().ToList();

        public IEnumerable<IFiftyOneAspectPropertyMetaData> GetProperties()
        {
            using (var properties =
                _engine.MetaData.getPropertiesForComponent(_source))
            {
                for (uint i = 0; i < properties.getSize(); i++)
                {
                    yield return new PropertyMetaDataHash(
                        _engine,
                        properties.getByIndex(i));
                }
            }
        }

        public IFiftyOneAspectPropertyMetaData GetProperty(string propertyName)
        {
            IFiftyOneAspectPropertyMetaData result = null;
            using (var properties =
                _engine.MetaData.getPropertiesForComponent(_source))
            {
                var property = properties.getByKey(propertyName);
                if (property != null)
                {
                    result = new PropertyMetaDataHash(_engine, property);
                }
            }
            return result;
        }

        public string Name => _source.getName();

        public byte ComponentId => (byte)_source.getComponentIdAsInt();

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _source.Dispose();
            }
        }

        ~ComponentMetaData()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            return ComponentId;
        }

        public bool Equals(IComponentMetaData other)
        {
            return ComponentId.Equals(other.ComponentId);
        }

        public int CompareTo(IComponentMetaData other)
        {
            return ComponentId.CompareTo(other.ComponentId);
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
