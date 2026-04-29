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

using FiftyOne.DeviceDetection.RobotsTxt.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace FiftyOne.DeviceDetection.RobotsTxt.Tests.Services
{
    /// <summary>
    /// Tests for <see cref="TdlSourcesLoader"/>. The loader is the
    /// startup gate for tdlSources.json so the bar for validation
    /// failures throwing is high — they should fail fast rather than
    /// letting a broken config slip into runtime.
    /// </summary>
    [TestClass]
    public class TdlSourcesLoaderTests
    {
        private const string ValidJson = @"
[
  {
    ""Id"": ""MOW-SOCW"",
    ""Repository"": ""movementforanopenweb/terms-documents"",
    ""Directory"": ""socw"",
    ""FilePattern"": ""^(?<n>\\d+)\\.txt$"",
    ""UrlTemplate"": ""https://m4ow.uk/socw/{name}""
  }
]";

        /// <summary>
        /// The default config shipped with the engine should contain
        /// the canonical MOW-SOCW source out of the box.
        /// </summary>
        [TestMethod]
        public void LoadDefault_ReturnsEmbeddedSourcesWithMowSocw()
        {
            var result = TdlSourcesLoader.LoadDefault();

            Assert.IsTrue(result.ContainsKey("MOW-SOCW"));
            var entry = result["MOW-SOCW"];
            Assert.AreEqual("movementforanopenweb/terms-documents", entry.Repository);
            Assert.AreEqual("socw", entry.Directory);
            Assert.AreEqual("https://m4ow.uk/socw/{name}", entry.UrlTemplate);
        }

        /// <summary>
        /// Lookups against the embedded default ignore case so callers
        /// don't have to match the JSON's exact casing.
        /// </summary>
        [TestMethod]
        public void LoadDefault_LookupIsCaseInsensitive()
        {
            var result = TdlSourcesLoader.LoadDefault();

            Assert.IsTrue(result.ContainsKey("mow-socw"));
            Assert.IsTrue(result.ContainsKey("Mow-SoCw"));
        }

        /// <summary>
        /// Happy path for the file overload: a single well-formed
        /// entry yields a one-key dictionary with the original
        /// values intact.
        /// </summary>
        [TestMethod]
        public void LoadFromFile_ValidSource_ReturnsDictionaryKeyedById()
        {
            var path = System.IO.Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, ValidJson);

                var result = TdlSourcesLoader.LoadFromFile(path);

                Assert.HasCount(1, result);
                Assert.IsTrue(result.ContainsKey("MOW-SOCW"));
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Missing file is a deployment problem; surface a
        /// <see cref="FileNotFoundException"/> so the cause is obvious
        /// in startup logs.
        /// </summary>
        [TestMethod]
        public void LoadFromFile_FileMissing_ThrowsFileNotFound()
        {
            var path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                $"missing-{Guid.NewGuid():N}.json");

            Assert.ThrowsExactly<FileNotFoundException>(
                () => TdlSourcesLoader.LoadFromFile(path));
        }

        /// <summary>
        /// Empty path is a programmer error.
        /// </summary>
        [TestMethod]
        public void LoadFromFile_EmptyPath_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(
                () => TdlSourcesLoader.LoadFromFile(""));
        }

        /// <summary>
        /// Null input to the in-memory overload is a programmer error.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_NullJson_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(
                () => TdlSourcesLoader.LoadFromJson(null));
        }

        /// <summary>
        /// An empty array is a valid (if pointless) config and should
        /// not throw.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_EmptyArray_ReturnsEmptyDictionary()
        {
            var result = TdlSourcesLoader.LoadFromJson("[]");

            Assert.IsEmpty(result);
        }

        /// <summary>
        /// JSON that is not parseable should be wrapped in
        /// <see cref="InvalidOperationException"/> with the underlying
        /// JsonException kept as inner exception.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_MalformedJson_Throws()
        {
            var ex = Assert.ThrowsExactly<InvalidOperationException>(
                () => TdlSourcesLoader.LoadFromJson("{ not json"));
            Assert.IsNotNull(ex.InnerException);
        }

        /// <summary>
        /// Top-level JSON 'null' deserialises to a null list and is
        /// rejected with a clear message.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_JsonNullArray_Throws()
        {
            Assert.ThrowsExactly<InvalidOperationException>(
                () => TdlSourcesLoader.LoadFromJson("null"));
        }

        /// <summary>
        /// A null element inside the array is rejected — the index in
        /// the message helps the author find it.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_NullElement_Throws()
        {
            var ex = Assert.ThrowsExactly<InvalidOperationException>(
                () => TdlSourcesLoader.LoadFromJson("[null]"));
            StringAssert.Contains(ex.Message, "index 0");
        }

        /// <summary>
        /// Each required field must be non-empty; missing Id is the
        /// representative case.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_MissingId_Throws()
        {
            var json = @"
[
  {
    ""Repository"": ""x/y"",
    ""Directory"": ""d"",
    ""FilePattern"": ""^(?<n>\\d+)$"",
    ""UrlTemplate"": ""https://x/{name}""
  }
]";
            var ex = Assert.ThrowsExactly<InvalidOperationException>(
                () => TdlSourcesLoader.LoadFromJson(json));
            StringAssert.Contains(ex.Message, "Id");
        }

        /// <summary>
        /// A FilePattern that is not a compilable regex must surface
        /// the original ArgumentException as inner.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_InvalidRegex_Throws()
        {
            var json = @"
[
  {
    ""Id"": ""X"",
    ""Repository"": ""x/y"",
    ""Directory"": ""d"",
    ""FilePattern"": ""[unclosed"",
    ""UrlTemplate"": ""https://x/{name}""
  }
]";
            var ex = Assert.ThrowsExactly<InvalidOperationException>(
                () => TdlSourcesLoader.LoadFromJson(json));
            Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        }

        /// <summary>
        /// FilePattern without the named version group cannot be used
        /// to pick the latest file, so it is rejected.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_PatternMissingNamedGroup_Throws()
        {
            var json = @"
[
  {
    ""Id"": ""X"",
    ""Repository"": ""x/y"",
    ""Directory"": ""d"",
    ""FilePattern"": ""^\\d+\\.txt$"",
    ""UrlTemplate"": ""https://x/{name}""
  }
]";
            var ex = Assert.ThrowsExactly<InvalidOperationException>(
                () => TdlSourcesLoader.LoadFromJson(json));
            StringAssert.Contains(ex.Message, "named group");
        }

        /// <summary>
        /// Without the {name} placeholder the loader cannot build the
        /// final URL, so we reject the entry rather than silently
        /// returning a malformed link at runtime.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_UrlTemplateMissingPlaceholder_Throws()
        {
            var json = @"
[
  {
    ""Id"": ""X"",
    ""Repository"": ""x/y"",
    ""Directory"": ""d"",
    ""FilePattern"": ""^(?<n>\\d+)$"",
    ""UrlTemplate"": ""https://x/static.txt""
  }
]";
            var ex = Assert.ThrowsExactly<InvalidOperationException>(
                () => TdlSourcesLoader.LoadFromJson(json));
            StringAssert.Contains(ex.Message, "{name}");
        }

        /// <summary>
        /// Two entries with the same Id (even differing in case) are
        /// ambiguous; reject so the author resolves the conflict.
        /// </summary>
        [TestMethod]
        public void LoadFromJson_DuplicateId_Throws()
        {
            var json = @"
[
  {
    ""Id"": ""X"",
    ""Repository"": ""a/b"",
    ""Directory"": ""d"",
    ""FilePattern"": ""^(?<n>\\d+)$"",
    ""UrlTemplate"": ""https://x/{name}""
  },
  {
    ""Id"": ""x"",
    ""Repository"": ""c/d"",
    ""Directory"": ""d"",
    ""FilePattern"": ""^(?<n>\\d+)$"",
    ""UrlTemplate"": ""https://y/{name}""
  }
]";
            var ex = Assert.ThrowsExactly<InvalidOperationException>(
                () => TdlSourcesLoader.LoadFromJson(json));
            StringAssert.Contains(ex.Message, "Duplicate");
        }

    }
}
