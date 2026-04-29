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

using System;

namespace FiftyOne.DeviceDetection.RobotsTxt.Services;

/// <summary>
/// Plug-in for <see cref="FlowElements.RobotsTxtEngine"/> that maps a
/// short TDL identifier (e.g. "MOW-SOCW") to the current best URL for
/// that source. The engine calls into this for any TDL evidence value
/// that is not already an absolute URI; consumers that don't register
/// a resolver get the default "URI-only" behaviour.
/// </summary>
public interface ITdlSourceResolver
{
    /// <summary>
    /// Returns true if <paramref name="id"/> is a configured source.
    /// Comparison is case-insensitive.
    /// </summary>
    bool IsKnown(string id);

    /// <summary>
    /// Resolves a known <paramref name="id"/> to the latest available
    /// URL. Returns null if the id is unknown, or if the live lookup
    /// failed and there is no cached value to fall back on.
    /// </summary>
    Uri Resolve(string id);
}
