using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace SharpWebServer
{
    public class Request
    {
        public HttpListenerContext Context;
        public IPAddress RemoteAddress;
        public string Method;
        public NameValueCollection Params;
        public byte[] Body;
        public NameValueCollection Headers;
        public string Path;
        public string[] Groups;
        public Session Session;
        public Server Server;

        public Request()
        {

        }

        public Session CreateSession()
        {
            Session session = Server.Sessions.Create();
            Session = session;
            Context.Response.AddHeader("Set-Cookie", $"SESSION={session.Token}; Path=/;");
            return session;
        }

        public void DestroySession()
        {
            if (Session != null)
            {
                Session = null;
                Context.Response.AddHeader("Set-Cookie", $"SESSION=0; Path=/; Max-Age=0;");
            }
        }

        public void SetContentType(string contentType)
        {
            this.Context.Response.Headers.Set("Content-Type", contentType);
        }

        public void SetStatus(int code, string description = "")
        {
            this.Context.Response.StatusCode = code;
            this.Context.Response.StatusDescription = description;
        }

        public void Respond(byte[] bytes)
        {
            try
            {
                if (this.Context.Request.Headers.AllKeys.Contains("Accept-Encoding") &&
                this.Context.Request.Headers.Get("Accept-Encoding").Contains("deflate"))
                {
                    this.Context.Response.Headers.Set("Content-Encoding", "deflate");
                    bytes = Utils.Deflate(bytes);
                }

                this.Context.Response.ContentLength64 = bytes.Length;

                Stream responseStream = new MemoryStream(bytes);
                byte[] buffer = new byte[2048];
                int nbytes;
                while ((nbytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    this.Context.Response.OutputStream.Write(buffer, 0, nbytes);
                this.Context.Response.OutputStream.Close();
            }
            catch (Exception)
            {
            }
        }

        public void Respond(string text)
        {
            Respond(Encoding.UTF8.GetBytes(text));
        }

        public void StreamMedia(string filePath)
        {
            // stream audio and video files with support for range requests
            SetStatus(206);
            Context.Response.AddHeader("Accept-Ranges", "bytes");

            int read;
            byte[] buffer = new byte[4096];

            using (Stream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                Context.Response.ContentLength64 = stream.Length;

                using (BinaryWriter writer = new BinaryWriter(Context.Response.OutputStream))
                {
                    if (!String.IsNullOrEmpty(Context.Request.Headers["Range"]))
                    {
                        string[] range = Context.Request.Headers["Range"].Split(new char[] { '=', '-' });
                        int beginPos = int.Parse(range[1]);
                        int endPos = int.Parse(range[1]);

                        stream.Seek(beginPos, SeekOrigin.Begin);
                        Context.Response.AddHeader("Content-Range", String.Format("bytes {0}-{1}/{2}", beginPos, stream.Length - 1, stream.Length));
                    }

                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, buffer.Length);
                        writer.Flush();
                    }
                }
            }
        }
    }
}
