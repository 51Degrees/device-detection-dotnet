/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2026 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Data
{
    /// <summary>
    /// A wrapper around <see cref="IFiftyOneAspectPropertyMetaData"/>
    /// that re-associates the property with the correct outer engine 
    /// rather than the internal DeviceDetectionHashEngine. Can also
    /// create a synthetic property with explicit name, type, and 
    /// item properties.
    /// </summary>
    public class DevicePropertyMetaData : IFiftyOneAspectPropertyMetaData
    {
        private readonly IFiftyOneAspectPropertyMetaData _inner;

        private readonly string _name;
        private readonly Type _type;
        private readonly IReadOnlyList<IElementPropertyMetaData> 
            _itemProperties;

        /// <inheritdoc/>
        public IFlowElement Element { get; }

        // --- IElementPropertyMetaData ---

        /// <inheritdoc/>
        public string Name => _inner != null ? _inner.Name : _name;

        /// <inheritdoc/>
        public string Category => _inner?.Category ?? string.Empty;

        /// <inheritdoc/>
        public Type Type => _inner != null ? _inner.Type : _type;

        /// <inheritdoc/>
        public bool Available => _inner?.Available ?? true;

        /// <inheritdoc/>
        public IReadOnlyList<IElementPropertyMetaData> ItemProperties =>
            _inner != null
                ? _inner.ItemProperties
                : (_itemProperties ?? 
                    (IReadOnlyList<IElementPropertyMetaData>)
                    new List<IElementPropertyMetaData>());

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, IElementPropertyMetaData> 
            ItemPropertyDictionary => 
            ItemProperties.ToDictionary(p => p.Name, p => p);

        /// <inheritdoc/>
        public bool DelayExecution => _inner?.DelayExecution ?? false;

        /// <inheritdoc/>
        public IReadOnlyList<string> EvidenceProperties => 
            _inner?.EvidenceProperties ?? 
            (IReadOnlyList<string>)new List<string>();

        // --- IAspectPropertyMetaData ---

        /// <inheritdoc/>
        public IList<string> DataTiersWherePresent => 
            _inner?.DataTiersWherePresent ?? new List<string>();

        /// <inheritdoc/>
        public string Description => _inner?.Description ?? string.Empty;

        // --- IFiftyOneAspectPropertyMetaData ---

        /// <inheritdoc/>
#pragma warning disable CA1056
        public string Url => _inner?.Url ?? string.Empty;
#pragma warning restore CA1056

        /// <inheritdoc/>
        public byte DisplayOrder => _inner?.DisplayOrder ?? 0;

        /// <inheritdoc/>
        public bool Mandatory => _inner?.Mandatory ?? false;

        /// <inheritdoc/>
        public bool List => _inner?.List ?? false;

        /// <inheritdoc/>
        public bool Obsolete => _inner?.Obsolete ?? false;

        /// <inheritdoc/>
        public bool Show => _inner?.Show ?? true;

        /// <inheritdoc/>
        public bool ShowValues => _inner?.ShowValues ?? true;

        /// <inheritdoc/>
#pragma warning disable CA1721
        public IComponentMetaData Component => _inner?.Component;
#pragma warning restore CA1721

        /// <inheritdoc/>
#pragma warning disable CA1721
        public IReadOnlyList<IValueMetaData> Values => 
            _inner?.Values ?? 
            (IReadOnlyList<IValueMetaData>)new List<IValueMetaData>();
#pragma warning restore CA1721

        /// <inheritdoc/>
#pragma warning disable CA1721
        public IValueMetaData DefaultValue => _inner?.DefaultValue;
#pragma warning restore CA1721

        /// <summary>
        /// Creates a wrapper with a different associated engine.
        /// </summary>
        /// <param name="element">
        /// The engine to associate with this property.
        /// </param>
        /// <param name="source">
        /// The original property metadata from the inner engine.
        /// </param>
        public DevicePropertyMetaData(
            IFlowElement element,
            IFiftyOneAspectPropertyMetaData source)
        {
            Element = element;
            _inner = source;
        }

        /// <summary>
        /// Creates a new synthetic property metadata with explicit name, 
        /// type, and item properties.
        /// </summary>
        /// <param name="element">
        /// The engine to associate with this property.
        /// </param>
        /// <param name="name">
        /// The property name (e.g. "Profiles").
        /// </param>
        /// <param name="itemProperties">
        /// Sub-properties of this property.
        /// </param>
        /// <param name="type">
        /// The CLR type of this property.
        /// </param>
        public DevicePropertyMetaData(
            IFlowElement element,
            string name,
            IReadOnlyList<IFiftyOneAspectPropertyMetaData> itemProperties,
            Type type)
        {
            Element = element;
            _inner = null;
            _name = name;
            _itemProperties = itemProperties
                .Cast<IElementPropertyMetaData>().ToList();
            _type = type;
        }

        // --- IFiftyOneAspectPropertyMetaData methods ---

        /// <inheritdoc/>
        [System.Obsolete("Will be removed in a future version")]
        public IComponentMetaData GetComponent()
        {
            return Component;
        }

        /// <inheritdoc/>
        public IEnumerable<IValueMetaData> GetValues()
        {
            return Values;
        }

        /// <inheritdoc/>
        public IValueMetaData GetValue(string valueName)
        {
            return _inner?.GetValue(valueName);
        }

        /// <inheritdoc/>
        [System.Obsolete("Will be removed in a future version")]
        public IValueMetaData GetDefaultValue()
        {
            return DefaultValue;
        }

        // --- IEquatable / IComparable / IDisposable ---

        /// <inheritdoc/>
        public bool Equals(IFiftyOneAspectPropertyMetaData other)
        {
            if (other == null) return false;
            return Name == other.Name && Element == other.Element;
        }

        /// <inheritdoc/>
        public bool Equals(string other)
        {
            return Name == other;
        }

        /// <inheritdoc/>
        public int CompareTo(IFiftyOneAspectPropertyMetaData other)
        {
            if (other == null) return 1;
            return string.Compare(Name, other.Name, 
                StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Nothing to dispose — this is a lightweight wrapper.
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is IFiftyOneAspectPropertyMetaData other)
            {
                return Equals(other);
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (Name?.GetHashCode() ?? 0) ^
                   (Element?.GetHashCode() ?? 0);
        }
    }
}
