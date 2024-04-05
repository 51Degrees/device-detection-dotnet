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

using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.Uach
{
    /// <summary>
    /// This is an <see cref="IFlowElement"/> implementation that converts evidence values
    /// from the User-Agent Client Hints JavaScript API method 
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/NavigatorUAData/getHighEntropyValues">getHighEntropyValues</see>
    /// to the HTTP header formatted evidence values that can be consumed by the main
    /// 51Degrees device detection API.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/pipeline-elements/uach-high-entropy-decoder.md#element-data">Specification</see>
    /// </summary>
    public class UachJsConversionElement : 
        FlowElementBase<UachJsConversionData, IElementPropertyMetaData>, IFlowElement
    {
        public override string ElementDataKey => "uach-js-conversion";

        private EvidenceKeyFilterWhitelist _evidenceKeyFilter;

        public override IEvidenceKeyFilter EvidenceKeyFilter => _evidenceKeyFilter;

        public override IList<IElementPropertyMetaData> Properties => new List<IElementPropertyMetaData>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="elementDataFactory"></param>
        public UachJsConversionElement(
            ILogger<FlowElementBase<UachJsConversionData, IElementPropertyMetaData>> logger, 
            Func<IPipeline, FlowElementBase<UachJsConversionData, IElementPropertyMetaData>, UachJsConversionData> elementDataFactory) 
            : base(logger, elementDataFactory)
        {
            _evidenceKeyFilter = new EvidenceKeyFilterWhitelist(
                new List<string>() {
                    Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE,
                    Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_QUERY
                });
        }

        /// <summary>
        /// Called by <see cref="FlowElementBase{T, TMeta}.Process(IFlowData)"/> to execute 
        /// this element's processing logic.
        /// </summary>
        /// <param name="data">
        /// The active <see cref="IFlowData"/> instance.
        /// </param>
        protected override void ProcessInternal(IFlowData data)
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string queryEvidence;
            string cookieEvidence = null;
            // Check for entries under the evidence keys we're going to work with.
            if (data.TryGetEvidence(Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_QUERY, out queryEvidence) ||
                data.TryGetEvidence(Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE, out cookieEvidence))
            {
                string encodedValue;
                string usedEvidenceKey;
                if (queryEvidence != null)
                {
                    encodedValue = queryEvidence;
                    usedEvidenceKey = Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_QUERY;
                }
                else
                {
                    encodedValue = cookieEvidence;
                    usedEvidenceKey = Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE;
                }
                UachJson uachData = null;
                try
                {
                    // Decode from base 64
                    byte[] bytes = Convert.FromBase64String(encodedValue);
                    string decodedValue = Encoding.UTF8.GetString(bytes);

                    // De-serialize to object
                    uachData = JsonConvert.DeserializeObject<UachJson>(decodedValue);
                }
                catch (FormatException)
                { ThrowException(usedEvidenceKey); }
                catch (JsonReaderException)
                { ThrowException(usedEvidenceKey); }

                if (uachData != null)
                {
                    bool evidenceAdded = false;
                    // Populate the evidence values for each of the UACH properties.
                    var brandsString = SerializeBrands(uachData.Brands);
                    if (brandsString != null)
                    {
                        data.AddEvidence(Shared.Constants.EVIDENCE_SECCHUA_HEADER_KEY, brandsString);
                        evidenceAdded = true;
                    }
                    var brandsFullVersionString = SerializeBrands(uachData.FullVersionList);
                    if (brandsFullVersionString != null)
                    {
                        data.AddEvidence(Shared.Constants.EVIDENCE_SECCHUA_FULLVERSIONLIST_HEADER_KEY,
                            brandsFullVersionString);
                        evidenceAdded = true;
                    }

                    if (uachData.Mobile.HasValue)
                    {
                        data.AddEvidence(Shared.Constants.EVIDENCE_SECCHUA_MOBILE_HEADER_KEY,
                            uachData.Mobile.Value ? "?1" : "?0");
                        evidenceAdded = true;
                    }

                    if (uachData.Model != null)
                    {
                        data.AddEvidence(Shared.Constants.EVIDENCE_SECCHUA_MODEL_HEADER_KEY,
                            ConvertText(uachData.Model));
                        evidenceAdded = true;
                    }
                    if (uachData.Platform != null)
                    {
                        data.AddEvidence(Shared.Constants.EVIDENCE_SECCHUA_PLATFORM_HEADER_KEY,
                            ConvertText(uachData.Platform));
                        evidenceAdded = true;
                    }
                    if (uachData.PlatformVersion != null)
                    {
                        data.AddEvidence(Shared.Constants.EVIDENCE_SECCHUA_PLATFORM_VERSION_HEADER_KEY,
                            ConvertText(uachData.PlatformVersion));
                        evidenceAdded = true;
                    }

                    if (evidenceAdded == false)
                    {
                        ThrowException(usedEvidenceKey);
                    }
                }
            }
        }

        /// <summary>
        /// Throw an exception about the format of the evidence value
        /// </summary>
        /// <param name="usedEvidenceKey">
        /// The key of the evidence value that was processed
        /// </param>
        private static void ThrowException(string usedEvidenceKey)
        {
            throw new PipelineException(string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                Messages.ExceptionUachJsUnexpectedFormat, usedEvidenceKey));
        }

        /// <summary>
        /// Serialize the specified list of brand objects to a string that matches the
        /// format of the UA-CH headers.
        /// </summary>
        /// <param name="brands"></param>
        /// <returns></returns>
        private static string SerializeBrands(IList<UachJsonBrand> brands)
        {
            string result = null;
            if (brands != null && brands.Count > 0)
            {
                result = string.Join(", ", 
                    brands.Where(i => i != null)
                        .Select(i => $"{ConvertText(i.Brand)};v={ConvertText(i.Version)}"));
            }
            return result;
        }

        /// <summary>
        /// Convert the specified text to the format used in UA-CH headers.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string ConvertText(string text)
        {
            return $"\"{text}\"";
        }

        protected override void ManagedResourcesCleanup()
        {
        }
        protected override void UnmanagedResourcesCleanup()
        {
        }
    }
}
