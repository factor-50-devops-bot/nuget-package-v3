using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelpMyStreet.Utils.Utils;
using NUnit.Framework;

namespace HelpMyStreet.UnitTests
{
    public class CompressionUtilsTests
    {
        [Test]
        public void TestString()
        {
            string input = "eyup!";
            byte[] compressedData = CompressionUtils.Gzip(input);
            string uncompressedData = CompressionUtils.UnGzipToString(compressedData);
            Assert.AreEqual(input, uncompressedData);
        }

        [Test]
        public void TestBytes()
        {
            string input = "eyup!";

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            byte[] compressedData = CompressionUtils.Gzip(inputBytes);
            byte[] uncompressedData = CompressionUtils.UnGZipToBytes(compressedData);
            
            Assert.IsTrue(inputBytes.SequenceEqual(uncompressedData));
        }

        [Test]
        public void IsGZipped()
        {
            string input = "eyup!";

            byte[] compressedData = CompressionUtils.Gzip(input);

            Assert.IsTrue(CompressionUtils.IsGZipped(compressedData));
        }

        [Test]
        public void IsnotGZipped()
        {
            string input = "eyup!";

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            Assert.IsFalse(CompressionUtils.IsGZipped(inputBytes));
        }
    }
}
