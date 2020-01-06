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

using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Data
{
    public class ProfileMetaData : IProfileMetaData, IDisposable
    {
        internal readonly DeviceDetectionPatternEngine _engine;
        internal readonly ProfileMetaDataSwig _source;

        internal ProfileMetaData(
            DeviceDetectionPatternEngine engine, 
            ProfileMetaDataSwig source)
        {
            _source = source;
            _engine = engine;
        }

        public uint ProfileId => _source.getProfileId();

        public uint SignatureCount => _source.getSignatureCount();

        public IEnumerable<IValueMetaData> GetValues()
        {
            using (var values = _engine.MetaData.getValuesForProfile(_source))
            {
                for (uint i = 0; i < values.getSize(); i++)
                { 
                    yield return new ValueMetaData(_engine, values.getByIndex(i));
                }
            }
        }

        public IComponentMetaData Component => new ComponentMetaData(
            _engine, 
            _engine.MetaData.getComponentForProfile(_source));

        public string Name
        {
            get
            {
                var name = String.Join("/",
                    GetValues().Where(i =>
                    i.Property.DisplayOrder > 0 && i.Name != "N/A").OrderByDescending(i =>
                    i.Property.DisplayOrder).Select(i =>
                    i.Name).Distinct()).Trim();

                if (string.IsNullOrEmpty(name))
                    return ProfileId.ToString();
                else
                    return name;
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _source.Dispose();
            }
        }

        ~ProfileMetaData()
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
            return (int)ProfileId;
        }

        public bool Equals(IProfileMetaData other)
        {
            return ProfileId.Equals(other.ProfileId);
        }

        public int CompareTo(IProfileMetaData other)
        {
            return ProfileId.CompareTo(other.ProfileId);
        }

        public IEnumerable<IValueMetaData> GetValues(string propertyName)
        {
            using (var values = _engine.MetaData.getValuesForProfile(_source))
            {
                for (uint i = 0; i < values.getSize(); i++)
                {
                    var value = new ValueMetaData(_engine, values.getByIndex(i));
                    if (propertyName.Equals(value.Property.Name))
                    {
                        yield return value;
                    }
                    else
                    {
                        value.Dispose();
                    }
                }
            }
        }

        public IValueMetaData GetValue(string propertyName, string valueName)
        {
            IValueMetaData result = null;
            using (var values = _engine.MetaData.getValuesForProfile(_source))
            {
                var value = values.getByKey(new ValueMetaDataKeySwig(
                    propertyName,
                    valueName));
                if (value != null)
                {
                    result = new ValueMetaData(_engine, value);
                }
            }
            return result;
        }

        #endregion
    }
}