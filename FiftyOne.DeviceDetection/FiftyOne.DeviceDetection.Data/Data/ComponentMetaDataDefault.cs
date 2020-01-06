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
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.Shared.Data
{
    /// <summary>
    /// Default implementation of IComponentMetaData
    /// </summary>
    public abstract class ComponentMetaDataDefault : IComponentMetaData
    {
        private List<IFiftyOneAspectPropertyMetaData> _properties;

        public byte ComponentId => 255;

        public string Name { get; }

        public virtual IProfileMetaData DefaultProfile => null;

        public IReadOnlyList<IFiftyOneAspectPropertyMetaData> Properties => GetProperties().ToList();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public ComponentMetaDataDefault(string name) : this(name, new List<IFiftyOneAspectPropertyMetaData>())
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        public ComponentMetaDataDefault(string name, List<IFiftyOneAspectPropertyMetaData> properties)
        {
            Name = name;
            _properties = properties;
        }

        public void AddProperty(IFiftyOneAspectPropertyMetaData property)
        {
            if(Properties.Count(p => p.Name == property.Name) == 0)
                _properties.Add(property);
        }

        public int CompareTo(IComponentMetaData other)
        {
            return ComponentId.CompareTo(other.ComponentId);
        }

        public void Dispose()
        {
            
        }

        public bool Equals(IComponentMetaData other)
        {
            return ComponentId.Equals(other.ComponentId);
        }

        public IEnumerable<IFiftyOneAspectPropertyMetaData> GetProperties()
        {
            return _properties;
        }

        public IFiftyOneAspectPropertyMetaData GetProperty(string propertyName)
        {
            return _properties.SingleOrDefault(p => p.Name == propertyName);
        }

        public override int GetHashCode()
        {
            return ComponentId;
        }
    }
}
