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
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.Engines.FlowElements;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    /// <summary>
    /// Data class that contains meta-data relating to a specific 
    /// property. 
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/data-model-specification/README.md#property">Specification</see>
    /// </summary>
    public class FiftyOneAspectPropertyMetaDataHash : FiftyOneAspectPropertyMetaDataDefault
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="category"></param>
        /// <param name="dataTiersWherePresent"></param>
        /// <param name="available"></param>
        /// <param name="component"></param>
        /// <param name="defaultValue"></param>
        /// <param name="values"></param>
        /// <param name="description"></param>
        public FiftyOneAspectPropertyMetaDataHash(
            IAspectEngine element,
            string name,
            Type type,
            string category,
            IList<string> dataTiersWherePresent,
            bool available,
            ComponentMetaDataDefault component,
            ValueMetaDataDefault defaultValue,
            IEnumerable<ValueMetaDataDefault> values,
            string description)
            : this(element,
                name,
                type,
                category,
                dataTiersWherePresent,
                available,
                component,
                defaultValue,
                values,
                description,
                255,
                false, false, false, true, true, string.Empty)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="category"></param>
        /// <param name="dataTiersWherePresent"></param>
        /// <param name="available"></param>
        /// <param name="component"></param>
        /// <param name="defaultValue"></param>
        /// <param name="values"></param>
        /// <param name="description"></param>
        /// <param name="displayOrder"></param>
        /// <param name="list"></param>
        /// <param name="mandatory"></param>
        /// <param name="obsolete"></param>
        /// <param name="show"></param>
        /// <param name="showValues"></param>
        /// <param name="url"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1054:Uri parameters should not be strings",
            Justification = "This would be a breaking change that we " +
            "will not be making at this time")]
        public FiftyOneAspectPropertyMetaDataHash(
            IAspectEngine element,
            string name,
            Type type,
            string category,
            IList<string> dataTiersWherePresent,
            bool available,
            ComponentMetaDataDefault component,
            ValueMetaDataDefault defaultValue,
            IEnumerable<ValueMetaDataDefault> values,
            string description,
            byte displayOrder,
            bool list,
            bool mandatory,
            bool obsolete,
            bool show,
            bool showValues,
            string url) : base(element, name, type, category, dataTiersWherePresent, available, component, defaultValue, values, description, displayOrder, list, mandatory,obsolete, show, showValues, url)
        { }
    }
}
