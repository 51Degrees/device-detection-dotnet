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

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    /// <summary>
    /// A single captured log call.
    /// </summary>
    public sealed class CapturedLog
    {
        public CapturedLog(LogLevel level, string category, Exception exception)
        {
            Level = level;
            Category = category;
            Exception = exception;
        }

        public LogLevel Level { get; }
        public string Category { get; }
        public Exception Exception { get; }
    }

    /// <summary>
    /// Minimal <see cref="ILoggerProvider"/> that records every log call so a
    /// test can assert on the level and exception that were logged. Used to
    /// verify that client-caused validation errors are not logged at Error
    /// level (which could be surfaced as exception telemetry).
    /// </summary>
    public sealed class CapturingLoggerProvider : ILoggerProvider
    {
        private readonly List<CapturedLog> _entries = new List<CapturedLog>();
        private readonly object _lock = new object();

        public IReadOnlyList<CapturedLog> Entries
        {
            get { lock (_lock) { return _entries.ToList(); } }
        }

        public void Clear()
        {
            lock (_lock) { _entries.Clear(); }
        }

        public ILogger CreateLogger(string categoryName) =>
            new CapturingLogger(categoryName, _entries, _lock);

        public void Dispose() { }

        private sealed class CapturingLogger : ILogger
        {
            private readonly string _category;
            private readonly List<CapturedLog> _entries;
            private readonly object _lock;

            public CapturingLogger(
                string category, List<CapturedLog> entries, object lockObj)
            {
                _category = category;
                _entries = entries;
                _lock = lockObj;
            }

            public IDisposable BeginScope<TState>(TState state) =>
                NullScope.Instance;

            // Always enabled so a regression that re-introduces the Error log
            // is captured rather than filtered out.
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception exception,
                Func<TState, Exception, string> formatter)
            {
                lock (_lock)
                {
                    _entries.Add(
                        new CapturedLog(logLevel, _category, exception));
                }
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();
            public void Dispose() { }
        }
    }
}
