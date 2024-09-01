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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    /// <summary>
    /// Data class that contains meta-data relating to a specific 
    /// profile. 
    /// A profile is a set of specific property values for an individual 
    /// component.
    /// For example, the Apple iPhone 8 has a profile that contains
    /// values for all the hardware component properties.
    /// This implementation of the <see cref="IProfileMetaData"/> interface
    /// is used for meta-data that is generated by the native C/C++ code
    /// from information in the data file.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
        "CA1036:Override methods on comparable types",
        Justification = "Mathematical operator-style methods are not " +
        "appropriate for this class.")]
    public class ProfileMetaData : IProfileMetaData, IDisposable
    {
        internal readonly DeviceDetectionHashEngine _engine;
        internal readonly ProfileMetaDataSwig _source;

        internal ProfileMetaData(
            DeviceDetectionHashEngine engine, 
            ProfileMetaDataSwig source)
        {
            _source = source;
            _engine = engine;
        }

        /// <summary>
        /// Get the id for this profile. 
        /// The id number is guaranteed to stay the same for a profile
        /// regardless of data updates and API version changes.
        /// </summary>
        public uint ProfileId => _source.getProfileId();

        /// <summary>
        /// Get the meta-data for all values associated with this profile. 
        /// </summary>
        /// <returns>
        /// The meta-data for all values associated with this profile.
        /// </returns>
        public IEnumerable<IValueMetaData> GetValues()
        {
            using (var values = _engine.MetaData.getValuesForProfile(_source))
            {
                for (uint i = 0; i < values.getSize(); i++)
                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                    // The ValueMetaDataSwig instance is used and disposed 
                    // by the ValueMetaData object.
                    yield return new ValueMetaData(_engine, values.getByIndex(i));
#pragma warning restore CA2000 // Dispose objects before losing scope
                }
            }
        }

        /// <summary>
        /// Get the meta-data for the component associated with this profile.
        /// </summary>
        public IComponentMetaData Component => new ComponentMetaData(
            _engine, 
            _engine.MetaData.getComponentForProfile(_source));

        /// <summary>
        /// Get the name of this profile.
        /// </summary>
        public string Name
        {
            get
            {
                var name = String.Join("/",
                    GetValues().Where(i =>
                    i.GetProperty().DisplayOrder > 0 && i.Name != "N/A").OrderByDescending(i =>
                    i.GetProperty().DisplayOrder).Select(i =>
                    i.Name).Distinct()).Trim();

                if (string.IsNullOrEmpty(name))
                    return ProfileId.ToString(CultureInfo.InvariantCulture);
                else
                    return name;
            }
        }

        #region IDisposable Support
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">
        /// False if called from finalizer
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _source.Dispose();
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ProfileMetaData()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get a hash code for this profile
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)ProfileId;
        }

        /// <summary>
        /// Check if this instance is equal to another object that 
        /// implements <see cref="IProfileMetaData"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="IProfileMetaData"/> to check for equality
        /// </param>
        /// <returns>
        /// True if the two instances are equal.
        /// False otherwise
        /// </returns>
        public bool Equals(IProfileMetaData other)
        {
            if (other == null) return false;
            return ProfileId.Equals(other.ProfileId);
        }

        /// <summary>
        /// Compare this instance to another object that implements 
        /// <see cref="IProfileMetaData"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="IProfileMetaData"/> instance to compare to.
        /// </param>
        /// <returns>
        /// &gt;0 if this instance precedes `other` in the sort order.
        /// 0 if they are equal in the sort order.
        /// &lt;0 if `other` precedes this instance in the sort order.
        /// </returns>
        public int CompareTo(IProfileMetaData other)
        {
            if (other == null) return -1;
            return ProfileId.CompareTo(other.ProfileId);
        }

        /// <summary>
        /// Get the meta-data for values from this profile for the 
        /// specified property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get values for.
        /// </param>
        /// <returns>
        /// The value meta-data for the specified property on this profile. 
        /// </returns>
        public IEnumerable<IValueMetaData> GetValues(string propertyName)
        {
            using (var values = _engine.MetaData.getValuesForProfile(_source))
            {
                for (uint i = 0; i < values.getSize(); i++)
                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                    // This instance will be returned by this method so
                    // we don't want to dispose of it.
                    var value = new ValueMetaData(_engine, values.getByIndex(i));
#pragma warning restore CA2000 // Dispose objects before losing scope
                    using (var valueProperty = value.GetProperty()) 
                    {
                        if (propertyName.Equals(valueProperty.Name,
                            StringComparison.OrdinalIgnoreCase))
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
        }

        /// <summary>
        /// Get the meta-data for the specified property and value from
        /// this profile.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get the meta-data for.
        /// </param>
        /// <param name="valueName">
        /// The value to get the meta-data for.
        /// </param>
        /// <returns>
        /// The value meta-data for the specified value if it exists
        /// on this profile. 
        /// </returns>
        public IValueMetaData GetValue(string propertyName, string valueName)
        {
            IValueMetaData result = null;
            using (var values = _engine.MetaData.getValuesForProfile(_source))
            {
                using (var key = new ValueMetaDataKeySwig(
                    propertyName,
                    valueName))
                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                    // The ValueMetaDataSwig instance is used and disposed 
                    // by the ValueMetaData object.
                    var value = values.getByKey(key);
#pragma warning restore CA2000 // Dispose objects before losing scope
                    if (value != null)
                    {
                        result = new ValueMetaData(_engine, value);
                    }
                }
            }
            return result;
        }

        #endregion
    }
}