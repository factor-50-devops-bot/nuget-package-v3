using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace HelpMyStreet.Utils.Utils
{
    public class CompressionUtils
    {
        public static byte[] _gzipHeaderBytes = { 31, 139, 8 };

        public static byte[] Gzip(byte[] bytes)
        {
            using (MemoryStream msi = new MemoryStream(bytes))
            using (MemoryStream mso = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }
        
        public static byte[] UnGZipToBytes(byte[] bytes)
        {
            using (MemoryStream msi = new MemoryStream(bytes))
            using (MemoryStream mso = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                return mso.ToArray();
            }
        }

        public static byte[] Gzip(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            return Gzip(bytes);
        }

        public static string UnGzipToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(UnGZipToBytes(bytes));
        }

        public static bool IsGZipped(byte[] bytes)
        {
            // gzip files have a 10 byte header that starts with 31 and 139. the third number 8 signifies the deflate compression method. https://tools.ietf.org/html/rfc1952#page-6

            bool yes = bytes.Length > 10; 

            if (!yes)
            {
                return false;
            }

            byte[] header = new byte[3];
            Array.Copy(bytes, 0, header, 0, 3);

            bool isGzipped = header.SequenceEqual(_gzipHeaderBytes);
            return isGzipped;
        }


        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
    }
}
