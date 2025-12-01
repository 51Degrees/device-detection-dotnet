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

using System;
using System.Collections.Generic;
using System.Linq;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    /// <summary>
    /// Data class that contains meta-data relating to a specific 
    /// component.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/data-model-specification/README.md#component">Specification</see>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
        "CA1036:Override methods on comparable types",
        Justification = "Mathematical operator-style methods are not " +
        "appropriate for this class.")]
    public class ComponentMetaData : IComponentMetaData, IDisposable
    {
        private readonly DeviceDetectionHashEngine _engine;

        private readonly ComponentMetaDataSwig _source;

        internal ComponentMetaData(
            DeviceDetectionHashEngine engine, 
            ComponentMetaDataSwig source)
        {
            _engine = engine;
            _source = source;
        }

        /// <summary>
        /// The default profile for this component.
        /// </summary>
        public IProfileMetaData DefaultProfile => new ProfileMetaData(
            _engine,
            _engine.MetaData.getDefaultProfileForComponent(_source));

        /// <summary>
        /// Get the meta-data for all properties associated with this 
        /// component.
        /// </summary>
        [Obsolete("Use the GetProperties method instead.")]
        public IReadOnlyList<IFiftyOneAspectPropertyMetaData> Properties =>
            GetProperties().ToList();


        /// <summary>
        /// Get the meta-data for all properties that are associated with 
        /// this component.
        /// </summary>
        /// <returns>
        /// Meta-data for all properties that are associated with this 
        /// component.
        /// </returns>
        public IEnumerable<IFiftyOneAspectPropertyMetaData> GetProperties()
        {
            using (var properties =
                _engine.MetaData.getPropertiesForComponent(_source))
            {
                for (uint i = 0; i < properties.getSize(); i++)
                {
                    yield return new PropertyMetaDataHash(
                        _engine,
#pragma warning disable CA2000 // Dispose objects before losing scope
                        // The 'PropertyMetaDataHash' instance uses the property
                        // object and will dispose of it when it is no longer
                        // needed.
                        properties.getByIndex(i));
#pragma warning restore CA2000 // Dispose objects before losing scope
                }
            }
        }

        /// <summary>
        /// Get the meta-data for the specified property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to get the meta-data for.
        /// </param>
        /// <returns>
        /// The meta-data for the specified property.
        /// </returns>
        public IFiftyOneAspectPropertyMetaData GetProperty(string propertyName)
        {
            IFiftyOneAspectPropertyMetaData result = null;
            using (var properties =
                _engine.MetaData.getPropertiesForComponent(_source))
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                // The 'PropertyMetaDataHash' instance uses the property
                // object and will dispose of it when it is no longer
                // needed.
                var property = properties.getByKey(propertyName);
#pragma warning restore CA2000 // Dispose objects before losing scope
                if (property != null)
                {
                    result = new PropertyMetaDataHash(_engine, property);
                }
            }
            return result;
        }

        /// <summary>
        /// The name of this component.
        /// </summary>
        public string Name => _source.getName();

        /// <summary>
        /// The id number for this component.
        /// </summary>
        public byte ComponentId => (byte)_source.getComponentIdAsInt();

        #region IDisposable Support
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">
        /// Will be false if called from finalizer
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
        ~ComponentMetaData()
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
        /// Get a hash code for this component
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ComponentId;
        }

        /// <summary>
        /// Check if this component is equal to another 
        /// <see cref="IComponentMetaData"/> instance.
        /// </summary>
        /// <param name="other">
        /// The other instance to check for equality.
        /// </param>
        /// <returns>
        /// True if the instances are equal. False if not.
        /// </returns>
        public bool Equals(IComponentMetaData other)
        {
            if (other == null) return false;
            return ComponentId.Equals(other.ComponentId);
        }
        
        /// <summary>
        /// Compare this instance to another object that implements 
        /// <see cref="IComponentMetaData"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="IComponentMetaData"/> instance to compare to.
        /// </param>
        /// <returns>
        /// &gt;0 if this instance precedes `other` in the sort order.
        /// 0 if they are equal in the sort order.
        /// &lt;0 if `other` precedes this instance in the sort order.
        /// </returns>
        public int CompareTo(IComponentMetaData other)
        {
            if (other == null) return -1;
            return ComponentId.CompareTo(other.ComponentId);
        }

        /// <summary>
        /// Return a string representation of this instance.
        /// </summary>
        /// <returns>
        /// A string representation of this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
