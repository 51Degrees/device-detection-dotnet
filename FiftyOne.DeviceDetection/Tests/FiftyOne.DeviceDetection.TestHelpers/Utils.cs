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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.TestHelpers
{
    public static class Utils
    {
        public static DirectoryInfo GetProjectRoot()
        {
            // Start with the DLL directory
            var current = new DirectoryInfo(Environment.CurrentDirectory);

            // Go up until we get to the bin directory. This is not
            // always the same number of steps, e.g. bin/Debug/ vs bin/x64/Debug
            while (current.Name != "bin")
            {
                current = current.Parent;
            }
            // Go up one more to get to the project
            current = current.Parent;

            return current;
        }

        private static bool IsSolutionDir(DirectoryInfo dir)
        {
            var files = dir.GetFiles("*.sln");
            return files.Any();
        }

        public static DirectoryInfo GetSolutionRoot()
        {
            var current = GetProjectRoot();
            while (IsSolutionDir(current) == false &&
                current.Parent != null)
            {
                current = current.Parent;
            }
            return current;
        }

        public static FileInfo GetFilePath(string filename)
        {
            // First look in the working directory
            var file = new FileInfo(Path.Combine(Environment.CurrentDirectory, filename));
            if (file.Exists)
            {
                Console.WriteLine($"Using data file '{file.FullName}'");
                return file;
            }
            // Now look in the solution directory.
            var searchRoot = GetSolutionRoot();
            var files = searchRoot.EnumerateFiles(filename,
                SearchOption.AllDirectories);

            file = files.FirstOrDefault();
            
            if (file.Exists)
            {
                Console.WriteLine($"Using data file '{file.FullName}'");
                return file;
            }
            Assert.Inconclusive($"Expected data file " +
                $"'{filename}' was missing. Test not run.");
            return null;
        }
    }
}
