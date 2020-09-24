using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SharpWebServer
{
    public static class Utils
    {
        private static Random random = new Random();

        public static byte[] GetRandomBytes(int length)
        {
            byte[] buffer = new byte[length];
            random.NextBytes(buffer);
            return buffer;
        }

        public static byte[] Deflate(byte[] bytes)
        {
            using (MemoryStream streamUncompressed = new MemoryStream(bytes))
            {
                using (MemoryStream streamCompressed = new MemoryStream())
                {
                    using (DeflateStream compressionStream = new DeflateStream(streamCompressed, CompressionMode.Compress))
                    {
                        streamUncompressed.CopyTo(compressionStream);
                    }

                    return streamCompressed.ToArray();
                }
            }
        }

        public static byte[] Inflate(byte[] bytes)
        {
            using (MemoryStream streamCompressed = new MemoryStream(bytes))
            {
                using (MemoryStream streamDecompressed = new MemoryStream())
                {
                    using (DeflateStream decompressionStream = new DeflateStream(streamCompressed, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(streamDecompressed);
                    }

                    return streamDecompressed.ToArray();
                }
            }
        }

        public static string RemoveQueryString(string url)
        {
            int i = url.IndexOf('?');
            if (i > -1)
            {
                return url.Remove(i);
            }
            return url;
        }

        public static string GetFileExtension(string url)
        {
            string[] parts = url.Split('/');

            if (parts.Length > 0)
            {
                string last = parts[parts.Length - 1];
                if (last.Contains("."))
                {
                    string[] _parts = last.Split('.');
                    if (_parts[1].Length > 0)
                    {
                        return _parts[1].ToLower();
                    }
                }
            }

            return null;
        }

        public static string RemoveDoubleSlashes(string url)
        {
            return url.Replace("//", "/");
        }

        public static string GetFileName(string url)
        {
            int i = url.LastIndexOf('/');
            if (i > -1) return url.Substring(i + 1);
            return null;
        }

        public static string RemoveFileName(string url)
        {
            int i = url.LastIndexOf('/');
            if (i > -1)
            {
                return url.Remove(i);
            }
            return url;
        }
    }
}
