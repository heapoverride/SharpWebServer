using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpWebServer
{
    public class Sessions
    {
        private Dictionary<string, Session> sessions = new Dictionary<string, Session>();

        public Sessions()
        {

        }

        public string Create()
        {
            string token = Hash.SHA256(Utils.GetRandomBytes(1024));
            sessions.Add(token, new Session(token));

            return token;
        }

        public bool Contains(string token)
        {
            return sessions.ContainsKey(token);
        }

        public Session Get(string token)
        {
            if (Contains(token))
            {
                return sessions[token];
            }
            return null;
        }

        public void Remove(string token)
        {
            if (Contains(token))
                sessions.Remove(token);
        }
    }

    public class Session
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();
        private string token = null;
        public string Token { get { return token; } }

        public Session(string token)
        {
            this.token = token;
        }

        public bool Contains(string name)
        {
            return storage.ContainsKey(name);
        }

        public void Set(string name, object value)
        {
            if (Contains(name))
            {
                storage[name] = value;
            }
            else
            {
                storage.Add(name, value);
            }
        }

        public object Get(string name)
        {
            if (Contains(name))
            {
                return storage[name];
            }
            return null;
        }

        public void Remove(string name)
        {
            if (Contains(name))
                storage.Remove(name);
        }
    }
}