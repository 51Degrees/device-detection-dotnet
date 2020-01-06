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

using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Shared.Data
{
    /// <summary>
    /// Default implementation of IValueMetaData
    /// </summary>
    public class ValueMetaDataDefault : IValueMetaData
    {
        IFiftyOneAspectPropertyMetaData _property;

        public IFiftyOneAspectPropertyMetaData Property => GetProperty();

        public string Name { get; }

        public string Description { get; }

        public string Url { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public ValueMetaDataDefault(string name)
            :this(name, string.Empty, string.Empty, null)
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        public ValueMetaDataDefault(string name, string description, string url)
            :this(name, description, url, null)
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        /// <param name="property"></param>
        public ValueMetaDataDefault(string name, string description, string url, IFiftyOneAspectPropertyMetaData property)
        {
            Name = name;
            Description = description;
            Url = url;
            _property = property;
        }

        public void SetProperty(IFiftyOneAspectPropertyMetaData property)
        {
            _property = property;
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

        public void Dispose()
        {}

        public bool Equals(IValueMetaData other)
        {
            return Name.Equals(other.Name);
        }

        public IFiftyOneAspectPropertyMetaData GetProperty()
        {
            return _property;
        }

        public override int GetHashCode()
        {
            return Property.GetHashCode() ^ Name.GetHashCode();
        }
    }
}
