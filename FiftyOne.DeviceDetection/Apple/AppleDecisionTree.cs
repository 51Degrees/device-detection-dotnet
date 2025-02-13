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


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// Data class used to store details of the decision tree used by 
    /// <see cref="AppleProfileEngine"/> in memory.
    /// 
    /// The decision tree is used to determine the profile ID of the device(s) that match the 
    /// supplied evidence values.
    /// </summary>
    public class AppleDecisionTree
    {
        /// <summary>
        /// The publish date of this decision tree
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// The version number of this decision tree
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// The root node for this decision tree
        /// </summary>
        public IAppleDecisionTreeNode Root { get; set; }

        /// <summary>
        /// Static factory method to create an instance from a stream containing a JSON 
        /// representation of the decision tree.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static AppleDecisionTree FromStream(Stream stream)
        {
            var tree = new AppleDecisionTree();

            // Parse the data from the stream.
            dynamic data = null;
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                data = serializer.Deserialize<dynamic>(jsonTextReader);
            }

            tree.Version = data.Version;
            tree.PublishDate = data.PublishDate;
            tree.Root = GetNode(data.Data, 0);

            return tree;
        }

        /// <summary>
        /// Get the specified node from the supplied dynamic object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static IAppleDecisionTreeNode GetNode(dynamic data, int index)
        {
            IAppleDecisionTreeNode result;
            var nodeData = data[index];

            // There is a 'v' property so create a standard node.
            if (PropertyExists(nodeData, "v"))
            {
                result = new AppleDecisionTreeNode()
                {
                    EvidenceValue = nodeData.v is JArray array ? 
                        array.First.ToString() : nodeData.v.ToString()
                };
            }
            // There is an 'r' property so create a range node.
            else if (PropertyExists(nodeData, "r"))
            {
                result = new AppleDecisionTreeNodeRange()
                {
                    EvidenceMin = nodeData.r[0].a,
                    EvidenceMax = nodeData.r[0].b
                };
            } 
            // If this is the root node (index == 0) then it won't have a 'v' or 'r' property.
            // Just create a standard node.
            else if (index == 0)
            {
                result = new AppleDecisionTreeNode();
            }
            else
            {
                throw new Exception($"No 'v' or 'r' property for this node " +
                    $"({Convert.ToString(data)})");
            }

            // Populate the common values
            result.EvidenceKey = PropertyExists(nodeData, "m") ? nodeData.m : null;
            result.ProfileId = PropertyExists(nodeData, "x") ? 
                (uint.TryParse(nodeData.x.ToString(), out uint idValue) ? (uint?)idValue : null) : null;

            // Populate child nodes
            if (PropertyExists(nodeData, "n"))
            {
                foreach (int childIndex in nodeData.n)
                {
                    result.AddChild(GetNode(data, childIndex));
                }
            }

            return result;
        }

        /// <summary>
        /// Return true if the specified property exists on the dynamic object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool PropertyExists(JObject obj, string name)
        {
            if (obj == null) return false;
            return obj.ContainsKey(name);
        }

    }
}
