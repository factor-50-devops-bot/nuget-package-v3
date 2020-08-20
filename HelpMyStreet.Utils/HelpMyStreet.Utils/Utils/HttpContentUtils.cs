using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HelpMyStreet.Utils.Utils
{
    public class HttpContentUtils
    {
        /// <summary>
        /// Serialises and compresses an object to StreamContent for use by HttpClient.  It uses Utf8Json for speed.
        /// </summary>
        /// <typeparam name="T">Type to serialise</typeparam>
        /// <param name="content">Content to Serialise</param>
        /// <returns></returns>
        public static StreamContent SerialiseToJsonAndCompress<T>(T content)
        {
            // Utf8Json is a much faster serialiser than Json.NET and System.Text.Json
            byte[] serialisedBytes = Utf8Json.JsonSerializer.Serialize(content);

            MemoryStream memoryStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(serialisedBytes, 0, serialisedBytes.Length);
            }

            memoryStream.Position = 0;
            StreamContent streamContent = new StreamContent(memoryStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            streamContent.Headers.ContentEncoding.Add("gzip");

            return streamContent;
        }

    }
}
