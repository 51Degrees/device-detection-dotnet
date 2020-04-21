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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    public class ValueMetaData : IValueMetaData, IDisposable
    {
        internal readonly ValueMetaDataSwig _source;
        internal readonly DeviceDetectionHashEngine _engine;

        internal ValueMetaData(DeviceDetectionHashEngine engine, ValueMetaDataSwig source)
        {
            _source = source;
            _engine = engine;
        }

        public IFiftyOneAspectPropertyMetaData Property => GetProperty();

        public string Name => _source.getName();

        public string Description => _source.getDescription();

        public string Url => _source.getUrl();

        public IFiftyOneAspectPropertyMetaData GetProperty()
        {
            return new PropertyMetaDataHash(
                _engine,
                _engine.MetaData.getPropertyForValue(_source));
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _source.Dispose();
            }
        }

        ~ValueMetaData()
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
            return Property.GetHashCode() ^ Name.GetHashCode();
        }

        public bool Equals(IValueMetaData other)
        {
            return Property.Equals(other.Property);
        }

        public int CompareTo(IValueMetaData other)
        {
            var difference = Property.CompareTo(other.Property);
            if (difference == 0)
            {
                difference = Name.CompareTo(other.Name);
            }
            return difference;
        }

        public override string ToString()
        {
            return $"{Property.Name}=>{Name}";
        }

        #endregion
    }
}
