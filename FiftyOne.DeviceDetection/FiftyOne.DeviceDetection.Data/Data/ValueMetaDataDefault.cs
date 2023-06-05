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

using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Shared.Data
{
    /// <summary>
    /// Data class that contains meta-data relating to a specific 
    /// value. 
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/data-model-specification/README.md#value">Specification</see>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
        "CA1036:Override methods on comparable types",
        Justification = "Mathematical operator-style methods are not " +
        "appropriate for this class.")]
    public class ValueMetaDataDefault : IValueMetaData
    {
        IFiftyOneAspectPropertyMetaData _property;

        /// <summary>
        /// Get the meta-data for the property associated with this 
        /// value instance.
        /// </summary>
        [Obsolete("Use the GetProperty method instead.")]
        public IFiftyOneAspectPropertyMetaData Property => GetProperty();

        /// <summary>
        /// The value
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of the value
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// URL that gives more information about the value.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// The value
        /// </param>
        public ValueMetaDataDefault(string name)
            :this(name, string.Empty, string.Empty, null)
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// The value
        /// </param>
        /// <param name="description">
        /// Description of the value
        /// </param>
        /// <param name="url">
        /// URL giving more information about the value
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1054:Uri parameters should not be strings",
            Justification = "This would be a breaking change that we " +
            "will not be making at this time")]
        public ValueMetaDataDefault(string name, string description, string url)
            :this(name, description, url, null)
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// The value
        /// </param>
        /// <param name="description">
        /// Description of the value
        /// </param>
        /// <param name="url">
        /// URL giving more information about the value
        /// </param>
        /// <param name="property">
        /// The meta-data for the property this value is for.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1054:Uri parameters should not be strings",
            Justification = "This would be a breaking change that we " +
            "will not be making at this time")]
        public ValueMetaDataDefault(string name, string description, 
            string url, IFiftyOneAspectPropertyMetaData property)
        {
            Name = name;
            Description = description;
            Url = url;
            _property = property;
        }

        /// <summary>
        /// Set the meta-data for the property that this value is for.
        /// </summary>
        /// <param name="property"></param>
        public void SetProperty(IFiftyOneAspectPropertyMetaData property)
        {
            _property = property;
        }

        /// <summary>
        /// Compare this instance with another object that implements 
        /// <see cref="IValueMetaData"/>
        /// </summary>
        /// <param name="other">
        /// The <see cref="IValueMetaData"/> to compare to
        /// </param>
        /// <returns>
        /// &gt;0 if this instance precedes `other` in the sort order.
        /// 0 if they are equal in the sort order.
        /// &lt;0 if `other` precedes this instance in the sort order.
        /// </returns>
        public int CompareTo(IValueMetaData other)
        {
            if (other == null) return -1;
            var difference = GetProperty().CompareTo(other.GetProperty());
            if (difference == 0)
            {
                difference = string.Compare(Name, other.Name, 
                    StringComparison.OrdinalIgnoreCase);
            }
            return difference;
        }

        /// <summary>
        /// Check if this instance is equal to another object that 
        /// implements <see cref="IValueMetaData"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="IValueMetaData"/> to check for 
        /// equality
        /// </param>
        /// <returns>
        /// True if the two instances are equal.
        /// False otherwise
        /// </returns>
        public bool Equals(IValueMetaData other)
        {
            if (other == null) return false;
            return GetProperty().Equals(other.GetProperty()) &&
                Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get the meta-data for the property that this value is for.
        /// </summary>
        /// <returns></returns>
        public IFiftyOneAspectPropertyMetaData GetProperty()
        {
            return _property;
        }

        /// <summary>
        /// Get the hash code for this instance.
        /// The value-space is greater than the key-space so hash 
        /// collisions are possible and may need to be accounted for,
        /// depending on your use-case.
        /// </summary>
        /// <returns>
        /// The hash code representing this value instance.
        /// </returns>
        public override int GetHashCode()
        {
            return GetProperty().GetHashCode() ^ Name.GetHashCode();
        }
        
        /// <summary>
        /// Check if this instance is equal to another object.
        /// </summary>
        /// <param name="obj">
        /// The object to check for equality
        /// </param>
        /// <returns>
        /// True if the two instances are equal.
        /// False otherwise
        /// </returns>
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is IValueMetaData other)
            {
                result = Equals(other);
            }
            return result;
        }

        /// <summary>
        /// Returns a string representation of this value.
        /// </summary>
        /// <returns>
        /// A string representation of this value.
        /// </returns>
        public override string ToString()
        {
            return GetProperty().ToString() + " = " + Name;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">
        /// False if called from the finalizer
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
