using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace SharpWebServer
{
    public static class Utils
    {
        private static Random random = new Random();

        public static string SHA256(byte[] input)
        {
            SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] inputBytes = input;
            byte[] hash = sha256.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }

        public static byte[] GetRandomBytes(int length)
        {
            byte[] buffer = new byte[length];
            random.NextBytes(buffer);
            return buffer;
        }

        public static string GetMimeType(string extension)
        {
            if (extension != null)
            {
                extension = extension.ToLower();

                for (int i = 0; i < mimeTypes.Length; i++)
                {
                    if (mimeTypes[i, 0] == extension)
                        return mimeTypes[i, 1];
                }
            }

            return "application/octet-stream";
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
            if (url == null) 
                return null;

            string[] parts = url.Split('/');

            if (parts.Length > 0)
            {
                string last = parts[parts.Length - 1];
                int index = last.LastIndexOf('.');
                if (index > -1)
                {
                    return last.Substring(index+1);
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

        public static string[,] mimeTypes = new string[,] {
            { "htm", "text/html" },
            { "html", "text/html" },
            { "js", "text/javascript" },
            { "txt", "text/plain" },
            { "bmp", "image/bmp" },
            { "jpg", "image/jpeg" },
            { "jpeg", "image/jpeg" },
            { "png", "image/png" },
            { "css", "text/css" },
            { "aac", "audio/aac" },
            { "abw", "application/x-abiword" },
            { "arc", "application/x-freearc" },
            { "avi", "video/x-msvideo" },
            { "azw", "application/vnd.amazon.ebook" },
            { "bin", "application/octet-stream" },
            { "bz", "application/x-bzip" },
            { "bz2", "application/x-bzip2" },
            { "csh", "application/x-csh" },
            { "csv", "text/csv" },
            { "doc", "application/msword" },
            { "docx", "application/vnd.openxmlformats-" },
            { "eot", "application/vnd.ms-fontobject" },
            { "epub", "application/epub+zip" },
            { "gz", "application/gzip" },
            { "gif", "image/gif" },
            { "ico", "image/x-icon" },
            { "ics", "text/calendar" },
            { "jar", "application/java-archive" },
            { "json", "application/json" },
            { "jsonld", "application/ld+json" },
            { "mid", "audio/midi" },
            { "midi", "audio/midi" },
            { "mjs", "text/javascript" },
            { "mp3", "audio/mpeg" },
            { "mp4", "video/mp4" },
            { "mpeg", "video/mpeg" },
            { "mpkg", "application/vnd.apple.installer+xml" },
            { "odp", "application/vnd.oasis.opendocument.presentation" },
            { "ods", "application/vnd.oasis.opendocument.spreadsheet" },
            { "odt", "application/vnd.oasis.opendocument.text" },
            { "oga", "audio/ogg" },
            { "ogv", "video/ogg" },
            { "ogx", "application/ogg" },
            { "opus", "audio/opus" },
            { "otf", "font/otf" },
            { "pdf", "application/pdf" },
            { "php", "text/html" },
            { "ppt", "application/vnd.ms-powerpoint" },
            { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { "rar", "application/vnd.rar" },
            { "rtf", "application/rtf" },
            { "sh", "application/x-sh" },
            { "svg", "image/svg+xml" },
            { "swf", "application/x-shockwave-flash" },
            { "tar", "application/x-tar" },
            { "tif", "image/tiff" },
            { "tiff", "image/tiff" },
            { "ts", "video/mp2t" },
            { "ttf", "font/ttf" },
            { "vsd", "application/vnd.visio" },
            { "wav", "audio/wav" },
            { "weba", "audio/webm" },
            { "webm", "video/webm" },
            { "webp", "image/webp" },
            { "woff", "font/woff" },
            { "woff2", "font/woff2" },
            { "xhtml", "application/xhtml+xml" },
            { "xls", "application/vnd.ms-excel" },
            { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "xml", "application/xml" },
            { "xul", "application/vnd.mozilla.xul+xml" },
            { "zip", "application/zip" },
            { "7z", "application/x-7z-compressed" },
        };
    }
}
