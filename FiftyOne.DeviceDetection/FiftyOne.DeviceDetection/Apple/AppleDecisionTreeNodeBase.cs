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
