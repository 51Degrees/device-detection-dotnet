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

using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.DeviceDetection.TestHelpers.Data;
using System.Linq;

namespace FiftyOne.DeviceDetection.Hash.Tests.Data
{
    public class MetaDataHasher : IMetaDataHasher
    {
        public int HashProperties(int hash, IWrapper wrapper)
        {
            foreach (var property in wrapper.Properties
                    .Where((p, i) => { return i % 10 == 0; }))
            {
                hash ^= property.GetHashCode();

                foreach (var value in property.Values
                    .Where((v, i) => { return i % 10 == 0; }))
                {
                    hash ^= value.GetHashCode();
                }
                hash ^= property.Component.GetHashCode();
                if (property.DefaultValue != null)
                {
                    hash ^= property.DefaultValue.GetHashCode();
                }
            }
            return hash;
        }

        public int HashValues(int hash, IWrapper wrapper)
        {
            foreach (var value in wrapper.Values
                        .Where((v, i) => { return i % 100 == 0; }))
            {
                hash ^= value.GetHashCode();
                hash ^= value.GetProperty() == null ? 0 : value.GetProperty().GetHashCode();
            }
            return hash;
        }

        public int HashComponents(int hash, IWrapper wrapper)
        {
            foreach (var component in wrapper.Components)
            {
                hash ^= component.GetHashCode();
                foreach (var property in component.Properties.
                    Where((p, i) => { return i % 10 == 0; }))
                {
                    hash ^= property.GetHashCode();
                }
                hash ^= component.DefaultProfile == null ? 0 : component.DefaultProfile.GetHashCode();
            }
            return hash;
        }

        public int HashProfiles(int hash, IWrapper wrapper)
        {
            foreach (var profile in wrapper.Profiles
                        .Where((p, i) => { return i % 100 == 0; }))
            {
                hash ^= profile.GetHashCode();
                hash ^= profile.Component.GetHashCode();
                foreach (var value in profile.GetValues()
                    .Where((v, i) => { return i % 10 == 0; }))
                {
                    hash ^= value.GetHashCode();
                }
            }
            return hash;
        }
    }
}
