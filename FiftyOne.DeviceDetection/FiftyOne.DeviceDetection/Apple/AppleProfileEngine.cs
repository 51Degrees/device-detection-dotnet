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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Core.TypedMap;
using FiftyOne.Pipeline.Engines.Caching;
using FiftyOne.Pipeline.Engines.Configuration;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// This engine uses evidence gathered from client-side scripts to determine the hardware 
    /// profile id of the Apple device(s). 
    /// </summary>
    public class AppleProfileEngine : FiftyOneOnPremiseAspectEngineBase<IAppleProfileData>
    {
        private ReaderWriterLockSlim _decisionTreeLock = new ReaderWriterLockSlim();
        private AppleDecisionTree DecisionTree { get; set; }

        public DateTime DataPublishDate
        {
            get
            {
                return DecisionTree.PublishDate;
            }
        }

        public Version DataVersion
        {
            get
            {
                return DecisionTree.Version;
            }
        }

        public AppleProfileEngine(
            ILogger<FiftyOneOnPremiseAspectEngineBase<IAppleProfileData>> logger, 
            Func<IPipeline, FlowElementBase<IAppleProfileData, IFiftyOneAspectPropertyMetaData>, IAppleProfileData> aspectDataFactory, 
            string tempDataFilePath) : base(logger, aspectDataFactory, tempDataFilePath)
        {
        }

        public override string DataSourceTier => "N/A";

        public override string ElementDataKey => "apple-profile";

        public override IEvidenceKeyFilter EvidenceKeyFilter =>
            new EvidenceKeyFilterWhitelist(Constants.EVIDENCE_APPLE_KEYS);

        public override IList<IFiftyOneAspectPropertyMetaData> Properties => 
            new List<IFiftyOneAspectPropertyMetaData>()
        {
            new ApplePropertyMetaData(this, "ProfileId", typeof(uint), "None", 
                "The hardware profile id of the Apple device(s) determined from the supplied evidence")
        };

        public override IEnumerable<IProfileMetaData> Profiles => null;

        public override IEnumerable<IComponentMetaData> Components => null;

        public override IEnumerable<IValueMetaData> Values => null;

        public override string GetDataDownloadType(string identifier)
        {
            return "N/A";
        }

        public override void RefreshData(string dataFileIdentifier)
        {
            var dataFile = DataFiles.Single();
            // Read the file.
            using (var stream = File.OpenRead(dataFile.DataFilePath))
            {
                RefreshData(dataFileIdentifier, stream);
            }
        }

        public override void RefreshData(string dataFileIdentifier, Stream data)
        {
            // Read the tree data from the stream.
            var newTree = AppleDecisionTree.FromStream(data);
            // Replace the existing decision tree with the new one.
            _decisionTreeLock.EnterWriteLock();
            try
            {
                DecisionTree = newTree;
            }
            finally
            {
                _decisionTreeLock.ExitWriteLock();
            }
        }

        protected override void ProcessEngine(IFlowData data, IAppleProfileData aspectData)
        {
            if (data == null) { throw new ArgumentNullException(nameof(data)); }
            if (aspectData == null) { throw new ArgumentNullException(nameof(aspectData)); }

            // Verify that cloud request engine or device detection engine comes after
            // this in the pipeline. If not, throw error explaining the problem.
            bool hasExpectedElement =
                data.Pipeline.HasExpectedElementAfter<AppleProfileEngine, CloudRequestEngine>() ||
                data.Pipeline.HasExpectedElementAfter<AppleProfileEngine, DeviceDetectionHashEngine>();
            if(hasExpectedElement == false)
            {
                throw new PipelineConfigurationException(Messages.ExceptionAppleEngineConfiguration);
            }

            // Determine matching profile ID using decision tree.
            var profileId = GetProfileId(data);

            // Set the profile id on the output data object.
            aspectData.ProfileId = profileId;

            // Also add the id as evidence so that the device detection engine can
            // use the value to override the hardware profile result.
            if (profileId.HasValue)
            {
                AddToEvidence(data, profileId.Value);
            }
        }

        /// <summary>
        /// Add the specified profile id to evidence
        /// </summary>
        /// <param name="data"></param>
        /// <param name="profileId"></param>
        private void AddToEvidence(IFlowData data, uint profileId)
        {
            var evidence = data.GetEvidence().AsDictionary();
            if (evidence.ContainsKey(Shared.Constants.EVIDENCE_PROFILE_IDS_KEY) == false)
            {
                data.AddEvidence(Shared.Constants.EVIDENCE_PROFILE_IDS_KEY,
                    profileId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                // There is already an evidence value for profile ids. Log a warning about this.
                Logger.LogWarning(string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    Messages.MessageAppleEvidenceConflict,
                    Shared.Constants.EVIDENCE_PROFILE_IDS_KEY));
            }
        }

        /// <summary>
        /// Get the matching profile id from the decision tree based on the evidence values in the 
        /// supplied <see cref="IFlowData"/> instance.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>
        /// The profile id. Null is returned if no evidence was provided or no match was found.
        /// </returns>
        private uint? GetProfileId(IFlowData data)
        {
            uint? result = null;

            _decisionTreeLock.EnterReadLock();
            try
            {
                bool evaluationComplete = false;
                var evidence = data.GetEvidence().AsDictionary();
                // Start the evaluation at the root node
                IAppleDecisionTreeNode node = DecisionTree.Root;

                while (evaluationComplete == false)
                {
                    // Evaluate evidence values to identify the next node to move to.
                    var nextNode = GetNextNode(node, evidence);
                    if (nextNode != null)
                    {
                        // If we find a match then advance to that node and repeat the process.
                        node = nextNode;
                    }
                    else
                    {
                        // If we don't find any matching nodes then we've gone as far as we can.
                        evaluationComplete = true;
                    }
                }
                result = node.ProfileId;
            }
            finally
            {
                _decisionTreeLock.ExitReadLock();
            }

            return result;

        }

        /// <summary>
        /// Find the next node in the evaluation tree based on the supplied evidence
        /// If no valid next node is found, this returns null.
        /// </summary>
        /// <param name="node">
        /// The current node.
        /// </param>
        /// <param name="evidence">
        /// All evidence values.
        /// </param>
        /// <returns></returns>
        private static IAppleDecisionTreeNode GetNextNode(
            IAppleDecisionTreeNode node, 
            IReadOnlyDictionary<string, object> evidence)
        {
            // If this node has an evidence key set, then get the evidence value for that key.
            if (node.EvidenceKey != null)
            {
                var evidenceKey = Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
                    Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
                    Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX +
                    node.EvidenceKey;
                if (evidence.TryGetValue(evidenceKey, out var evidenceValue))
                {
                    // Check the evidence value against each of the children.
                    foreach (var child in node.Children)
                    {
                        if (child.Match(evidenceValue))
                        {
                            // Return the matching child node
                            return child;
                        }
                    }
                }
            }

            // We've not managed to find a match so return null
            return null;
        }

        protected override void UnmanagedResourcesCleanup() { }
        protected override void ManagedResourcesCleanup()
        {
            DecisionTree = null;
            _decisionTreeLock.Dispose();
            base.ManagedResourcesCleanup();
        }
    }
}
