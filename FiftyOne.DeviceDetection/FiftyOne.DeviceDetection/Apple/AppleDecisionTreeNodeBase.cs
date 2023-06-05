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

using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// Base class for nodes in the decision tree used by <see cref="AppleDecisionTree"/>.
    /// </summary>
    public abstract class AppleDecisionTreeNodeBase : IAppleDecisionTreeNode
    {
        /// <summary>
        /// The hardware profile Id to return if the evaluation reaches this node but matches no 
        /// further children.
        /// </summary>
        public uint? ProfileId { get; set; }

        /// <summary>
        /// The key of the evidence value to use to evaluate against the child nodes.
        /// </summary>
        public string EvidenceKey { get; set; }

        private List<IAppleDecisionTreeNode> _children = new List<IAppleDecisionTreeNode>();
        /// <summary>
        /// The child nodes
        /// </summary>
        public IReadOnlyList<IAppleDecisionTreeNode> Children { get => _children; }

        /// <summary>
        /// The method to use to check if the supplied value matches this node.
        /// </summary>
        /// <param name="evidenceValue">
        /// The evidence value to compare against
        /// </param>
        /// <returns>
        /// True if the supplied evidence value matches the expected value for this node.
        /// </returns>
        public abstract bool Match(object evidenceValue);

        /// <summary>
        /// Add the specified node as a child.
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IAppleDecisionTreeNode child)
        {
            _children.Add(child);
        }
    }
}
