using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Apple
{
    public interface IAppleDecisionTreeNode
    {
        uint? ProfileId { get; set; }

        string EvidenceKey { get; set; }

        IReadOnlyList<IAppleDecisionTreeNode> Children { get; }

        bool Match(object evidenceValue);

        void AddChild(IAppleDecisionTreeNode child);
    }
}
