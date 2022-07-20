using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// A node in the decision tree that matches an evidence value against an expected range 
    /// of values.
    /// </summary>
    public class AppleDecisionTreeNodeRange : AppleDecisionTreeNodeBase
    {
        public float EvidenceMin { get; set; }

        public float EvidenceMax { get; set; }

        public override bool Match(object evidenceValue)
        {
            if(evidenceValue is float)
            {
                var floatValue = (float)evidenceValue;
                return floatValue >= EvidenceMin && floatValue <= EvidenceMax;
            }
            return false;
        }
    }
}
