using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// A node in the decision tree that matches an evidence value against a single expected value
    /// </summary>
    public class AppleDecisionTreeNode : AppleDecisionTreeNodeBase
    {
        public string EvidenceValue { get; set; }

        public override bool Match(object evidenceValue)
        {
            if (evidenceValue == null) { throw new ArgumentNullException(nameof(evidenceValue)); }
            return EvidenceValue.Equals(evidenceValue.ToString(), 
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
