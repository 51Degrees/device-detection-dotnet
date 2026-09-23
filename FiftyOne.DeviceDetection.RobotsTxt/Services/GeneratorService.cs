/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2026 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.DeviceDetection.RobotsTxt.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FiftyOne.DeviceDetection.RobotsTxt.Services;

public class GeneratorService(RobotsTxtModel _dataSet)
{
    /// <summary>
    /// Builds a robots.txt with or without annotations for the choices
    /// provided.
    /// </summary>
    /// <param name="writer">
    /// Text writer to send the robots.txt content to.
    /// </param>
    /// <param name="allowed">
    /// Usages that are allowed in the robots.txt. Crawlers that perform any
    /// of these usages are not given their own group and instead fall through
    /// to the wildcard Allow block (so TDL annotations on that block apply
    /// to them). A crawler the data records no usage for falls through when
    /// <see cref="Constants.NotApplicableUsage"/> is among them.
    /// </param>
    /// <param name="tdls">
    /// Terms Document Locator URIs. When non-empty, each URI is emitted as a
    /// TDL line (with a "# Terms ..." comment) inside the wildcard Allow
    /// block. When empty or null, the wildcard Allow block is emitted with
    /// no TDL lines.
    /// </param>
    /// <param name="annotations">
    /// True if the resulting file should include annotations, otherwise false.
    /// </param>
    /// <param name="stopToken"></param>
    public void Write(
        TextWriter writer,
        HashSet<string> allowed,
        IReadOnlyList<Uri> tdls,
        bool annotations,
        CancellationToken stopToken)
    {
        var disallowEntries = new Queue<string>();
        var allowedCrawlers = new List<CrawlerModel>();
        var notApplicableAllowed = GetIsNotApplicableAllowed(allowed);
        foreach (var crawler in _dataSet.Crawlers.OrderBy(i => i.Name))
        {
            if (GetIsAllowed(crawler, allowed, notApplicableAllowed) == false)
            {
                Add(disallowEntries, crawler, annotations ?
                    sb => AddAnnotations(crawler, sb) :
                    null);
            }
            else
            {
                allowedCrawlers.Add(crawler);
            }
        }

        // Add a legend for annotations.
        if (annotations)
        {
            AddHeader(writer);
        }
        else
        {
            AddCopyright(writer);
        }

        // Disallow blocks first, then the wildcard catch-all.
        while (disallowEntries.Count > 0)
        {
            writer.Write(disallowEntries.Dequeue());
            writer.WriteLine();
        }

        WriteWildcardBlock(writer, allowedCrawlers, tdls, annotations);
    }

    // Writes the wildcard catch-all that closes the file. It is always an Allow
    // block. In annotated output the crawlers that fall through to it (the
    // allowed crawlers) are listed as one comment group before the Allow record,
    // followed by any TDL lines. PlainText omits the crawler comments so it stays
    // a clean directive file.
    private void WriteWildcardBlock(
        TextWriter writer,
        IReadOnlyList<CrawlerModel> allowedCrawlers,
        IReadOnlyList<Uri> tdls,
        bool annotations)
    {
        writer.WriteLine("User-Agent: *");

        if (annotations)
        {
            var sb = new StringBuilder();
            foreach (var crawler in allowedCrawlers)
            {
                AddAnnotations(crawler, sb);
            }
            writer.Write(sb.ToString());
        }

        if (tdls != null && tdls.Count > 0)
        {
            foreach (var tdl in tdls)
            {
                var url = tdl.ToString();
                writer.Write("# Terms ");
                writer.WriteLine(url);
                writer.Write("TDL: ");
                writer.WriteLine(url);
            }
        }

        writer.WriteLine("Allow: /");
    }

    private void Add(
        Queue<string> entries,
        CrawlerModel crawler,
        Action<StringBuilder> addAnnotations)
    {
        var sb = new StringBuilder();

        // If annotations are enabled then add these for the entry.
        if (addAnnotations != null)
        {
            addAnnotations(sb);
        }

        // Where the tokens start, so that a crawler which contributed none
        // can be told apart from one that did without counting them first.
        var lengthBeforeTokens = sb.Length;

        // Add all the product tokens available.
        if (crawler.ProductTokens != null)
        {
            foreach (var token in crawler.ProductTokens)
            {
                // Only a token with something in it can head a group. A
                // parser matches a token as a substring of the crawler's
                // name, so an empty "User-Agent:" line matches every
                // crawler and the Disallow beneath it then refuses the
                // whole site to everybody.
                if (string.IsNullOrWhiteSpace(token))
                {
                    continue;
                }
                sb.AppendLine("User-Agent: " + token);
                sb.AppendLine("Disallow: /");
            }
        }

        // Nothing usable, so this crawler heads no group at all rather than
        // an empty one, and any annotations above go with it.
        if (sb.Length == lengthBeforeTokens)
        {
            return;
        }

        entries.Enqueue(sb.ToString());
    }

    /// <summary>
    /// Adds a simple copyright and license line at the top of the file. Needed
    /// to avoid competitors taking the data and using it in their products.
    /// </summary>
    /// <param name="sb"></param>
    private void AddCopyright(TextWriter sb)
    {
        sb.WriteLine("# robots.txt copyright 51Degrees. " +
            $"See {Constants.LicenseUrl} for details.");
    }

    /// <summary>
    /// Adds the header for the text with annotations to explain what the
    /// different entries mean.
    /// </summary>
    /// <param name="sb"></param>
    private static void AddHeader(TextWriter sb) 
    {
        // Add the license to the annotations header.
        using var reader = new StringReader(Constants.License);
        var line = reader.ReadLine();
        while (line != null)
        {
            sb.WriteLine("# " + line.Trim());
            line = reader.ReadLine();
        }
        sb.WriteLine();

        sb.WriteLine("# Legend");
        sb.WriteLine("# N: Name of the crawler");
        sb.WriteLine("# U: Usages that the crawler makes of obtained content");
        sb.WriteLine("# A: Address of any reference URLs available for more information");
        sb.WriteLine("# TDL: Terms Document Locator (immutable terms URL applied to the Allow block)");
        sb.WriteLine("# See https://51degrees.com/robots-txt for further details");
        sb.WriteLine();
    }

    private static void AddAnnotations(CrawlerModel crawler, StringBuilder sb)
    {
        sb.Append("# N: ").AppendLine(crawler.Name);
        sb.Append("# U: ").AppendLine(crawler.Usages == null ?
            String.Empty :
            String.Join(", ", crawler.Usages));
        if (crawler.ReferenceUris != null)
        {
            foreach (var uri in crawler.ReferenceUris)
            {
                sb.Append("# A: ").AppendLine(uri.ToString());
                break;
            }
        }
    }

    /// <summary>
    /// True if the crawler supports at least one of the allowed usages, or,
    /// where the data records no usage for it, if the N/A usage is allowed.
    /// </summary>
    /// <param name="crawler"></param>
    /// <param name="allowed"></param>
    /// <param name="notApplicableAllowed">
    /// True if the caller allows <see cref="Constants.NotApplicableUsage"/>,
    /// worked out once for the whole file by
    /// <see cref="GetIsNotApplicableAllowed(HashSet{string})"/>.
    /// </param>
    /// <returns></returns>
    private bool GetIsAllowed(
        CrawlerModel crawler,
        HashSet<string> allowed,
        bool notApplicableAllowed)
    {
        // A crawler the data records no usage for cannot be judged by usage,
        // and was refused however much the caller allowed. That refused 73
        // crawlers by name, among them Google AdExchange, Amazon CloudFront,
        // Datadog and Zabbix, in a file the caller had asked to allow
        // everything. It is governed instead by the N/A usage, the one
        // evidence key that names this case and which until now reached only
        // a crawler whose recorded usage value was itself N/A. See
        // 51Degrees/cloud issue 435.
        if (crawler.Usages == null
            || crawler.Usages.Any(i => string.IsNullOrWhiteSpace(i) == false) == false)
        {
            return notApplicableAllowed;
        }
        return crawler.Usages.Any(i => allowed.Contains(i));
    }

    /// <summary>
    /// True if the caller allows the N/A usage. Worked out once per file
    /// rather than per crawler, because it depends only on the allowed set.
    /// </summary>
    /// <param name="allowed"></param>
    /// <returns></returns>
    private static bool GetIsNotApplicableAllowed(HashSet<string> allowed)
    {
        // Compared without regard to case. The allowed set is built from the
        // usage names in the data file under the default ordinal comparer,
        // and only the lower cased form of the name reaches the caller, in
        // the evidence key, so the caller cannot be held to the data's case.
        return allowed.Any(i => string.Equals(
            i,
            Constants.NotApplicableUsage,
            StringComparison.OrdinalIgnoreCase));
    }
}
