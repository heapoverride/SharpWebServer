using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SharpWebServer
{
    public class Router
    {
        public List<Route> Routes = new List<Route>();

        public Router()
        {

        }

        public bool Route(ref Request request)
        {
            for (int i = Routes.Count - 1; i > -1; i--)
            {
                if (Routes[i].Test(ref request))
                    return true;
            }

            // route not found
            request.SetStatus(404, "Route not found");
            request.Respond("404 - Route not found");

            return false;
        }
    }

    public class Route
    {
        private string method = null;
        private string host = null;
        private Regex regex;
        private Action<Request> handler;

        public Route(string method, string host, string regex, Action<Request> handler)
        {
            this.regex = new Regex(regex, RegexOptions.Compiled);
            this.handler = handler;
        }

        public bool Test(ref Request request)
        {
            string hostHeader = request.Context.Request.Headers.Get("Host");

            if (!(host == null || hostHeader == host))
                return false;

            if (!(method == null || method.ToUpper() == request.Method))
                return false;

            Match match = this.regex.Match(request.Path);

            if (match.Success)
            {
                string[] parameters = null;

                if (match.Groups.Count > 0)
                {
                    parameters = new string[match.Groups.Count - 1];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = match.Groups[i + 1].Value;
                    }
                    request.Groups = parameters;
                }

                this.handler?.Invoke(request);
                return true;
            }

            return false;
        }
    }
}