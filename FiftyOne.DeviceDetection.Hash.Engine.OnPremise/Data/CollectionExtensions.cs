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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    internal static class CollectionExtensions
    {
        internal static IEnumerable<IComponentMetaData> Select(
            this ComponentMetaDataCollectionSwig source,
            Func<ComponentMetaDataSwig, IComponentMetaData> selector)
        {
            for (uint i = 0; i < source.getSize(); i++)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                // The IComponentMetaData instance that is created
                // will handle disposal
                yield return selector(source.getByIndex(i));
#pragma warning restore CA2000 // Dispose objects before losing scope
            }
        }

        internal static IEnumerable<IFiftyOneAspectPropertyMetaData> Select(
            this PropertyMetaDataCollectionSwig source,
            Func<PropertyMetaDataSwig, IFiftyOneAspectPropertyMetaData> selector)
        {
            for (uint i = 0; i < source.getSize(); i++)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                // The IFiftyOneAspectPropertyMetaData instance that is created
                // will handle disposal
                yield return selector(source.getByIndex(i));
#pragma warning restore CA2000 // Dispose objects before losing scope
            }
        }

        internal static IEnumerable<IProfileMetaData> Select(
            this ProfileMetaDataCollectionSwig source,
            Func<ProfileMetaDataSwig, IProfileMetaData> selector)
        {
            for (uint i = 0; i < source.getSize(); i++)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                // The IProfileMetaData instance that is created
                // will handle disposal
                yield return selector(source.getByIndex(i));
#pragma warning restore CA2000 // Dispose objects before losing scope
            }
        }

        internal static IEnumerable<IValueMetaData> Select(
            this ValueMetaDataCollectionSwig source,
            Func<ValueMetaDataSwig, IValueMetaData> selector)
        {
            for (uint i = 0; i < source.getSize(); i++)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                // The IValueMetaData instance that is created
                // will handle disposal
                yield return selector(source.getByIndex(i));
#pragma warning restore CA2000 // Dispose objects before losing scope
            }
        }
    }
}
