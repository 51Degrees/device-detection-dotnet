using FiftyOne.DeviceDetection.Apple;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FiftyOne.DeviceDetection.Tests.Core
{
    [TestClass]
    public class AppleDecisionTreeTests
    {
		public const string JSON_TEST_TREE = @"{
	""Version"": ""1.2"",
	""PublishDate"": ""2022-07-15T00:01:02.1234567Z"",
	""Data"": [{
			""m"": ""family"",
			""n"": [
				1,
				2
			]
		},
		{
			""m"": ""height"",
			""n"": [
				3
			],
			""v"": [
				""iPad""
			]
		},
		{
			""m"": ""width"",
			""n"": [
				4
			],
			""v"": [
				""iPhone""
			]
		},
		{
			""x"": 12345,
			""v"": [
				1024
			]
		},
		{
			""x"": 67890,
			""v"": [
				2048
			]
		}
	]}";

		/// <summary>
		/// Verify that the AppleDecisionTree.FromStream method loads the decision tree data
		/// correctly.
		/// </summary>
		[TestMethod]
        public void AppleDecisionTreeTest()
        {
			
			byte[] byteArray = Encoding.UTF8.GetBytes(JSON_TEST_TREE);
			using (var stream = new MemoryStream(byteArray))
			{
				var tree = AppleDecisionTree.FromStream(stream);

				// Verify tree metadata
				Assert.AreEqual(new Version(1, 2), tree.Version);
				Assert.AreEqual(2022, tree.PublishDate.Year);
				Assert.AreEqual(7, tree.PublishDate.Month);
				Assert.AreEqual(15, tree.PublishDate.Day);

				// Check root node values
				Assert.AreEqual("family", tree.Root.EvidenceKey);
				Assert.AreEqual(2, tree.Root.Children.Count);

				// Check first children
				Assert.IsNull(tree.Root.Children[0].ProfileId);
				Assert.AreEqual("height", tree.Root.Children[0].EvidenceKey);
				Assert.IsTrue(tree.Root.Children[0].Match("iPad"));
				Assert.IsFalse(tree.Root.Children[0].Match("iPhone"));
				Assert.IsNull(tree.Root.Children[1].ProfileId);
				Assert.AreEqual("width", tree.Root.Children[1].EvidenceKey);
				Assert.IsTrue(tree.Root.Children[1].Match("iPhone"));
				Assert.IsFalse(tree.Root.Children[1].Match("iPad"));

				// Check second children
				Assert.AreEqual(12345U, tree.Root.Children[0].Children[0].ProfileId);
				Assert.IsNull(tree.Root.Children[0].Children[0].EvidenceKey);
				Assert.IsTrue(tree.Root.Children[0].Children[0].Match(1024));
				Assert.IsTrue(tree.Root.Children[0].Children[0].Match("1024"));
				Assert.AreEqual(67890U, tree.Root.Children[1].Children[0].ProfileId);
				Assert.IsNull(tree.Root.Children[1].Children[0].EvidenceKey);
				Assert.IsTrue(tree.Root.Children[1].Children[0].Match(2048));
				Assert.IsTrue(tree.Root.Children[1].Children[0].Match("2048"));
			}
		}

    }
}
