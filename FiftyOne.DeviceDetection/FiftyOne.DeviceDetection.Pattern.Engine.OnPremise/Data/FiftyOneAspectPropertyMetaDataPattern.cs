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

using System;
using System.Collections.Generic;
using System.Text;
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FlowElements;

namespace FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Data
{
    public class FiftyOneAspectPropertyMetaDataPattern : FiftyOneAspectPropertyMetaDataDefault
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
        /// <param name="description"></param>
        public FiftyOneAspectPropertyMetaDataPattern(
            IAspectEngine element,
            string name,
            Type type,
            string category,
            IList<string> dataTiersWherePresent,
            bool available,
            IComponentMetaData component,
            IValueMetaData defaultValue,
            string description)
            : this(element,
                name,
                type,
                category,
                dataTiersWherePresent,
                available,
                component,
                defaultValue,
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
        /// <param name="description"></param>
        /// <param name="displayOrder"></param>
        /// <param name="list"></param>
        /// <param name="mandatory"></param>
        /// <param name="obsolete"></param>
        /// <param name="show"></param>
        /// <param name="showValues"></param>
        /// <param name="url"></param>
        public FiftyOneAspectPropertyMetaDataPattern(
            IAspectEngine element,
            string name,
            Type type,
            string category,
            IList<string> dataTiersWherePresent,
            bool available,
            IComponentMetaData component,
            IValueMetaData defaultValue,
            string description,
            byte displayOrder,
            bool list,
            bool mandatory,
            bool obsolete,
            bool show,
            bool showValues,
            string url) : base(element, name, type, category, dataTiersWherePresent, available, component, defaultValue,description, displayOrder, list, mandatory,obsolete, show, showValues, url)
        { }
    }
}
