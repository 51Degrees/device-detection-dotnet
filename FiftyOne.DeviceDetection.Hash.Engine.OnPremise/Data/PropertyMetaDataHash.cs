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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    /// <summary>
    /// Data class that contains meta-data relating to a specific 
    /// property. 
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/data-model-specification/README.md#property">Specification</see>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
        "CA1036:Override methods on comparable types",
        Justification = "Mathematical operator-style methods are not " +
        "appropriate for this class.")]
    public class PropertyMetaDataHash : IFiftyOneAspectPropertyMetaData, IDisposable
    {
        private readonly PropertyMetaDataSwig _source;
        private readonly DeviceDetectionHashEngine _engine;

        internal PropertyMetaDataHash(DeviceDetectionHashEngine engine, PropertyMetaDataSwig source)
        {
            _source = source;
            _engine = engine;

            // Load property meta data
            Url = _source.getUrl();
            DisplayOrder = (byte)_source.getDisplayOrder();
            Mandatory = _source.getIsMandatory();
            List = _source.getIsList();
            Obsolete = _source.getIsObsolete();
            Show = _source.getShow();
            ShowValues = _source.getShowValues();
            Description = _source.getDescription();
            DataTiersWherePresent = _source.getDataFilesWherePresent();
            Available = _source.getAvailable();
            Category = _source.getCategory();
            Name = _source.getName();
        }

        /// <summary>
        /// A URL that will display a page with more detail about the 
        /// property.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// A byte used to order property meta-data instances. For example
        /// when displaying a list of properties to a user.
        /// </summary>
        public byte DisplayOrder { get; }

        /// <summary>
        /// True if this property must be filled in. False if it can be 
        /// left blank or 'Unknown'
        /// </summary>
        public bool Mandatory { get; }

        /// <summary>
        /// True if this is a 'list' property. False if not.
        /// List properties can have multiple values on a single profile.
        /// For example, an Apple device such as the iPhone 8 has multiple 
        /// model numbers - A1863, A1864, A1897, etc. Therefore the 
        /// HardwareModelVariants property is a list property.
        /// </summary>
        public bool List { get; }

        /// <summary>
        /// True if the property is obsolete. False otherwise.
        /// Obsolete properties are usually retained for some time for
        /// backwards compatibility but may be removed in a future release 
        /// and should not be used if possible.
        /// </summary>
        public bool Obsolete { get; }

        /// <summary>
        /// True if 51Degrees recommends that the property should appear 
        /// in lists shown to the user. False if not.
        /// </summary>
        /// <remarks>
        /// This is used by the 51Degrees Property Dictionary: 
        /// https://51degrees.com/resources/property-dictionary and is 
        /// made available for customers should they wish to make use of it.
        /// </remarks>
        public bool Show { get; }

        /// <summary>
        /// True if 51Degrees recommends that all possible values of this 
        /// property can be shown to the user. False if not.
        /// Properties that have very large lists of possible values will
        /// usually have this set to false.
        /// </summary>
        /// <remarks>
        /// This is used by the 51Degrees Property Dictionary: 
        /// https://51degrees.com/resources/property-dictionary and is 
        /// made available for customers should they wish to make use of it.
        /// </remarks>
        public bool ShowValues { get; }

        /// <summary>
        /// A description of the property
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The component associated with this property.
        /// </summary>
        [Obsolete("Use the GetComponent method instead.")]
        // Marked obsolete, will be removed in a future version.
        public IComponentMetaData Component => GetComponent();

        /// <summary>
        /// The values that this property can have.
        /// Note that this is based on information in the data file so it
        /// is possible (or even likely, depending on the property) that a 
        /// future data file will have different possible values.
        /// </summary>
        [Obsolete("Use the GetValues method instead.")]
        public IReadOnlyList<IValueMetaData> Values => GetValues().ToList();

        /// <summary>
        /// Get the default value for this property
        /// </summary>
        [Obsolete("Use the GetDefaultValue method instead.")]
        public IValueMetaData DefaultValue => GetDefaultValue();

        /// <summary>
        /// A list of the string names of the data tiers that this property
        /// is available in.
        /// For example: 'Lite' or 'Enterprise' 
        /// </summary>
        public IList<string> DataTiersWherePresent { get; }

        /// <summary>
        /// True if this property is available from the current data source.
        /// False if not.
        /// </summary>
        public bool Available { get; }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The category that this property belongs to.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// The type of the values returned by this property.
        /// </summary>
        public Type Type
        {
            get
            {
                switch (_source.getType())
                {
                    case "string": return typeof(string);
                    case "int": return typeof(int);
                    case "bool": return typeof(bool);
                    case "double": return typeof(double);
                    case "javascript": return typeof(JavaScript);
                    case "string[]": return typeof(IReadOnlyList<string>);
                    default: return typeof(string);
                }
            }
        }

        /// <summary>
        /// Get the component associated with this property.
        /// </summary>
        /// <returns>
        /// The component associated with this property.
        /// </returns>
        public IComponentMetaData GetComponent()
        {
            return new ComponentMetaData(
                _engine,
                _engine.MetaData.getComponentForProperty(_source));
        }

        /// <summary>
        /// The values that this property can have.
        /// Note that this is based on information in the data file so it
        /// is possible (or even likely, depending on the property) that a 
        /// future data file will have different possible values.
        /// </summary>
        /// <returns>
        /// The values that this property can have.
        /// </returns>
        public IEnumerable<IValueMetaData> GetValues()
        {
            using (var values = _engine.MetaData.getValuesForProperty(_source, _engine))
            {
                foreach (var value in values)
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Get the meta-data for an individual value entry.
        /// </summary>
        /// <param name="valueName">
        /// the value to get the meta-data for.
        /// </param>
        /// <returns>
        /// The requested meta-data.
        /// </returns>
        public IValueMetaData GetValue(string valueName)
        {
            using (var values = _engine.MetaData.getValuesForProperty(_source, _engine))
            using (var key = new ValueMetaDataKeySwig(Name, valueName))
            {
                return new ValueMetaData(_engine,
                    values.getByKey(key));
            }
        }

        /// <summary>
        /// Get the default value for this property.
        /// </summary>
        /// <returns>
        /// A meta-data instance for the default value.
        /// </returns>
        public IValueMetaData GetDefaultValue()
        {
            var value = _engine.MetaData.getDefaultValueForProperty(_source);
            return value == null ? null : new ValueMetaData(_engine, value);
        }

        /// <summary>
        /// Check if this instance is equal to another object that 
        /// implements <see cref="IFiftyOneAspectPropertyMetaData"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="IFiftyOneAspectPropertyMetaData"/> to check 
        /// for equality.
        /// </param>
        /// <returns>
        /// True if the two instances are equal.
        /// False otherwise
        /// </returns>
        public bool Equals(IFiftyOneAspectPropertyMetaData other)
        {
            if (other == null) return false;
            return Name.Equals(other.Name, 
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if this instance is equal to a given string.
        /// This uses a comparison with the Name property.
        /// </summary>
        /// <param name="other">
        /// The string to check for equality.
        /// </param>
        /// <returns>
        /// True if the two instances are equal.
        /// False otherwise
        /// </returns>
        public bool Equals(string other)
        {
            if (other == null) return false;
            return Name.Equals(other, 
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get the element that created this meta-data instance.
        /// </summary>
        public IFlowElement Element => _engine;

        /// <summary>
        /// Get the meta-data for any sub-properties.
        /// </summary>
        public IReadOnlyList<IElementPropertyMetaData> ItemProperties { get; } = null;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, IElementPropertyMetaData> ItemPropertyDictionary => null;

        /// <inheritdoc/>
        public bool DelayExecution => false;

        /// <inheritdoc/>
        public IReadOnlyList<string> EvidenceProperties => null;

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
                _source?.Dispose();
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PropertyMetaDataHash()
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
        /// Compare this instance to another object that implements 
        /// <see cref="IFiftyOneAspectPropertyMetaData"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="IFiftyOneAspectPropertyMetaData"/> instance to 
        /// compare to.
        /// </param>
        /// <returns>
        /// &gt;0 if this instance precedes `other` in the sort order.
        /// 0 if they are equal in the sort order.
        /// &lt;0 if `other` precedes this instance in the sort order.
        /// </returns>
        public int CompareTo(IFiftyOneAspectPropertyMetaData other)
        {
            if (other == null) return -1;
            return string.Compare(Name, other.Name, 
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Return a string representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Check if this instance is equal to a given object.
        /// </summary>
        /// <param name="obj">
        /// The object to check for equality.
        /// </param>
        /// <returns>
        /// True if the two instances are equal.
        /// False otherwise
        /// </returns>
        public override bool Equals(object obj)
        {
            bool result = false;
            if(obj is IFiftyOneAspectPropertyMetaData metaData)
            {
                result = Equals(metaData);
            }
            else if (obj is string str)
            {
                result = Equals(str);
            }
            return result;
        }

        /// <summary>
        /// Get a hash code for this instance.
        /// The value-space is greater than the key-space so hash 
        /// collisions are possible and may need to be accounted for,
        /// depending on your use-case.
        /// </summary>
        /// <returns>
        /// A hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion
    }
}
