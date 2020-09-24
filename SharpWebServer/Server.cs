﻿using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace SharpWebServer
{
    public class Server
    {
        private HttpListener listener;
        private Action<Request> handler;
        private Router router;
        private Sessions sessions = new Sessions();
        public Sessions Sessions { get { return sessions; } }

        public Server(string[] prefixes, Action<Request> handler)
        {
            this.handler = handler;
            Listen(prefixes);
        }

        public Server(string[] prefixes, Router router)
        {
            this.router = router;
            Listen(prefixes);
        }

        private void Listen(string[] prefixes)
        {
            this.listener = new HttpListener();

            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }

            listener.Start();
            listener.BeginGetContext(new AsyncCallback(Callback), listener);
        }

        public void Stop()
        {
            listener.Abort();
        }

        public void Callback(IAsyncResult result)
        {
            try
            {
                listener.BeginGetContext(new AsyncCallback(Callback), listener);
                HttpListenerContext context = listener.EndGetContext(result);

                Process(context);
            }
            catch (Exception)
            {
            }
        }

        private Cookie GetCookie(HttpListenerContext context, string name)
        {
            foreach (Cookie cookie in context.Request.Cookies)
            {
                if (cookie.Name == name) return cookie;
            }
            return null;
        }

        private void Process(HttpListenerContext context)
        {
            try
            {
                IPEndPoint remoteEndPoint = context.Request.RemoteEndPoint;
                IPAddress remoteAddress = remoteEndPoint.Address;
                string method = context.Request.HttpMethod.ToUpper();
                NameValueCollection parameters = context.Request.QueryString;
                Stream inputStream = context.Request.InputStream;
                byte[] body = new byte[] { };
                if (method == "POST")
                {
                    body = Encoding.UTF8.GetBytes(new StreamReader(inputStream).ReadToEnd());

                    if (context.Request.Headers.AllKeys.Contains("Content-Encoding") &&
                        context.Request.Headers.Get("Content-Encoding").Contains("deflate"))
                    {
                        //decompress compressed request's bytes with Deflate
                        body = Utils.Inflate(body);
                    }
                }

                // session cookie
                Cookie session_cookie = GetCookie(context, "SESSION");
                Session session = session_cookie != null ? sessions.Get(session_cookie.Value) : null;
                if (session == null)
                {
                    string token = sessions.Create();
                    session = sessions.Get(token);

                    context.Response.Headers.Add("Set-Cookie", $"SESSION={token}; Path=/;");
                }

                Request request = new Request
                {
                    Context = context,
                    RemoteAddress = remoteAddress,
                    Method = method,
                    Params = parameters,
                    Body = body,
                    QueryString = context.Request.RawUrl.ToString(),
                    Headers = context.Request.Headers,
                    Session = session,
                };

                // set default content type
                request.SetContentType("text/html");

                if (this.router != null)
                {
                    router.Route(ref request);
                }
                else
                {
                    this.handler?.Invoke(request);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}