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

namespace FiftyOne.DeviceDetection.RobotsTxt.Model;

/// <summary>
/// Mapping entry that tells a resolver where to look on GitHub for the
/// versioned TDL files that belong to a short id, and how to format the
/// public URL once the latest file has been picked.
/// </summary>
public class TdlSource
{
    /// <summary>
    /// Name of the regex group in <see cref="FilePattern"/> that
    /// captures the version number used to pick the latest file.
    /// </summary>
    public const string VersionGroupName = "n";

    /// <summary>
    /// Placeholder in <see cref="UrlTemplate"/> swapped out for the
    /// winning file name.
    /// </summary>
    public const string NamePlaceholder = "{name}";

    /// <summary>
    /// Identifier callers send instead of a full URL (e.g. "MOW-SOCW").
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// GitHub repository in "owner/name" form.
    /// </summary>
    public string Repository { get; set; }

    /// <summary>
    /// Path inside the repository where the versioned files sit.
    /// </summary>
    public string Directory { get; set; }

    /// <summary>
    /// Regex over file names with a named group "n" capturing the
    /// version number. The highest n wins.
    /// </summary>
    public string FilePattern { get; set; }

    /// <summary>
    /// Public URL template; "{name}" is replaced by the winning
    /// file name.
    /// </summary>
    public string UrlTemplate { get; set; }
}
