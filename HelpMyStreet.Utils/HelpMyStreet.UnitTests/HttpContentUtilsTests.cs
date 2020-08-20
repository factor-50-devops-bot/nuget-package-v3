using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Utils;
using NUnit.Framework;

namespace HelpMyStreet.UnitTests
{
    public class HttpContentUtilsTests
    {
        [Test]
        public async Task SerialiseAndCompressContent()
        {
            TestObject testObject = new TestObject()
            {
                Id = 99
            };

            StreamContent result = HttpContentUtils.SerialiseToJsonAndCompress(testObject);

            Stream stream = await result.ReadAsStreamAsync();

            TestObject deserialisedAndDecompressedContent;
            using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                deserialisedAndDecompressedContent = await Utf8Json.JsonSerializer.DeserializeAsync<TestObject>(decompressionStream);
            }

            Assert.AreEqual(testObject.Id, deserialisedAndDecompressedContent.Id);
        }
    }

    [DataContract(Name = "testObject")]
    public class TestObject
    {
        [DataMember(Name = "i")]
        public int Id { get; set; }
    }
}
