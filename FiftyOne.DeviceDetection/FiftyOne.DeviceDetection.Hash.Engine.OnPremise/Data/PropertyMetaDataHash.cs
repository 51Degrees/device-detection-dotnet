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
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
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

        public string Url { get; }

        public byte DisplayOrder { get; }

        public bool Mandatory { get; }

        public bool List { get; }

        public bool Obsolete { get; }

        public bool Show { get; }

        public bool ShowValues { get; }

        public string Description { get; }

        public IComponentMetaData Component => GetComponent();

        public IReadOnlyList<IValueMetaData> Values => GetValues().ToList();

        public IValueMetaData DefaultValue => GetDefaultValue();

        public IList<string> DataTiersWherePresent { get; }

        public bool Available { get; }

        public string Name { get; }

        public string Category { get; }

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
                };
            }
        }

        public IComponentMetaData GetComponent()
        {
            return new ComponentMetaData(
                _engine,
                _engine.MetaData.getComponentForProperty(_source));
        }

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

        public IValueMetaData GetValue(string valueName)
        {
            using (var values = _engine.MetaData.getValuesForProperty(_source, _engine))
            {
                return new ValueMetaData(_engine,
                    values.getByKey(new ValueMetaDataKeySwig(Name, valueName)));
            }
        }

        public IValueMetaData GetDefaultValue()
        {
            var value = _engine.MetaData.getDefaultValueForProperty(_source);
            return value == null ? null : new ValueMetaData(_engine, value);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(IFiftyOneAspectPropertyMetaData other)
        {
            return Name.Equals(other.Name);
        }

        public bool Equals(string other)
        {
            return Name.Equals(other);
        }

        public IFlowElement Element => _engine;

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _source.Dispose();
            }
        }

        ~PropertyMetaDataHash()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int CompareTo(IFiftyOneAspectPropertyMetaData other)
        {
            return Name.CompareTo(other.Name);
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
