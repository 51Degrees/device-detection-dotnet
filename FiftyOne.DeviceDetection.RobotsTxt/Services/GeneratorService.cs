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
    /// Usages that are allowed in the robots.txt. All other usages will be
    /// disallowed.
    /// </param>
    /// <param name="annotations">
    /// True if the resulting file should include annotations, otherwise false.
    /// </param>
    /// <param name="stopToken"></param>
    public void Write(
        TextWriter writer,
        HashSet<string> allowed, 
        bool annotations, 
        CancellationToken stopToken)
    {
        var disallowEntries = new Queue<string>();
        var allowedEntries = new Queue<string>();
        foreach (var crawler in _dataSet.Crawlers.OrderBy(i => i.Name))
        {
            if (GetIsAllowed(crawler, allowed) == false)
            {
                // Nothing is allowed for this crawler so disallow it.
                Add(disallowEntries, crawler, false, annotations ?
                    sb => AddAnnotations(crawler, sb) : 
                    null);
            }
            else
            {
                // Get the crawler usages that are not allowed.
                var notAllowed = GetNotAllowed(crawler, allowed).ToArray();

                // If there are none then there is no problem. Add the crawler
                // as a normal entry.
                if (notAllowed.Length == 0)
                {
                    Add(allowedEntries, crawler, true, annotations ?
                        sb => AddAnnotations(crawler, sb) :
                        null);
                }

                // The crawler will perform some usages that are disallowed.
                // Add a warning to make it clear it's not possible in
                // robots.txt to prohibit some usages and not others.
                else
                {
                    Add(allowedEntries, crawler, true, annotations ?
                        sb => 
                        {
                            sb.Append("# WARNING - no restriction for - ");
                            sb.AppendLine(String.Join(", ", notAllowed));
                            AddAnnotations(crawler, sb);
                        } :
                        null);
                }
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

        // Write out the entries with allowed first, and then disallow.
        while (allowedEntries.Count > 0)
        {
            writer.Write(allowedEntries.Dequeue());
            writer.WriteLine();
        }
        while (disallowEntries.Count > 0)
        {
            writer.Write(disallowEntries.Dequeue());
            writer.WriteLine();
        }

        // Write out the catch all disallow all.
        AddFooter(writer);
    }

    private void Add(
        Queue<string> entries,
        CrawlerModel crawler,
        bool allowed,
        Action<StringBuilder> addAnnotations)
    {
        var sb = new StringBuilder();

        // If annotations are enabled then add these for the entry.
        if (addAnnotations != null)
        {
            addAnnotations(sb);
        }

        // Add all the product tokens available.
        foreach (var token in crawler.ProductTokens)
        {
            sb.AppendLine("User-Agent: " + token);
            sb.Append(allowed ? "Allow" : "Disallow").AppendLine(": /");
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
        sb.WriteLine("# See https://51degrees.com/robots-txt for further details");
        sb.WriteLine();
    }

    private static void AddFooter(TextWriter sb)
    {
        sb.WriteLine("User-Agent: *");
        sb.WriteLine("Disallow: /");
    }

    private static void AddAnnotations(CrawlerModel crawler, StringBuilder sb)
    {
        sb.Append("# N: ").AppendLine(crawler.Name);
        sb.Append("# U: ").AppendLine(String.Join(", ", crawler.Usages));
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
    /// True if the crawler supports at least one of the usages.
    /// </summary>
    /// <param name="crawler"></param>
    /// <param name="allowed"></param>
    /// <returns></returns>
    private bool GetIsAllowed(CrawlerModel crawler, HashSet<string> allowed)
    {
        return crawler.Usages.Any(i => allowed.Contains(i));
    }

    /// <summary>
    /// Returns any usages for the crawler that the allowed choices prohibit.
    /// </summary>
    /// <param name="crawler"></param>
    /// <param name="allowed"></param>
    /// <returns></returns>
    private IEnumerable<string> GetNotAllowed(
        CrawlerModel crawler,
        HashSet<string> allowed)
    {
        return crawler.Usages.Where(i => allowed.Contains(i) == false);
    }
}
