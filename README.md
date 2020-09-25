# SharpWebServer
Simple and small library built on top of HttpListener with 
fast routing and built-in session storage.

### Install
Copy the SharpWebServer folder to your solution and you're done.

### Example
```csharp
using System;
using System.Collections.Generic;
using SharpWebServer;

class Program {
    static Random random = new Random();

    static void Main(string[] args)
    {
        /* instantiate new router */
        Router router = new Router();


        /* 
         *  GET /
         */
        router.Routes.Add(new Route("GET", null, @"^\/$", request => {
            request.Respond("Hello world!");
        }));


        /* 
         *  GET /random/(min)/(max)
         */
        router.Routes.Add(new Route("GET", null, @"^\/random\/([1-9]{1}[0-9]*)\/([1-9]{1}[0-9]*)$", request => {
            int min = int.Parse(request.Groups[0]);
            int max = int.Parse(request.Groups[1]);

            request.SetContentType("text/plain");

            if (min <= max)
            {
                // respond with random number between (min) and (max)
                request.Respond(random.Next(min, max+1).ToString());
            } else
            {
                // error
                request.Respond("Error: (min) must be smaller or equal to (max)");
            }
        }));
        
        
        /* 
         *  GET /set/(value)
         */
        router.Routes.Add(new Route("GET", null, @"^\/set\/([^\/]*)$", request => {
            if (request.Session == null)
                request.CreateSession();

            string value = request.Groups[0];
            request.Session.Set("value", value);

            request.SetContentType("text/plain");
            request.Respond($"success: value set");
        }));

        /* 
         *  GET /get
         */
        router.Routes.Add(new Route("GET", null, @"^\/get$", request => {
            request.SetContentType("text/plain");

            if (request.Session != null)
            {
                object value = request.Session.Get("value");

                if (value != null)
                {
                    request.Respond($"value = {value}");
                }
                else
                {
                    request.Respond($"error: value is not set");
                }
            } else
            {
                request.Respond($"error: no session");
            }
        }));
        
        
        /* 
         *  GET /add/(item)
         */
        router.Routes.Add(new Route("GET", null, @"^\/add/([^\/]*)$", request => {
            string item = request.Groups[0];

            if (request.Session == null)
            {
                request.CreateSession()
                    .Set("firstName", "Robert")
                    .Set("lastName", "Robertson")
                    .Set("cartItems", new List<string>());
            }

            List<string> cartItems = request.Session.Get<List<string>>("cartItems");
            cartItems.Add(item);

            JArray array = new JArray(cartItems.ToArray());

            request.SetContentType("application/json");
            request.Respond(array.ToString());
        }));
        

        /* instantiate new server */
        string[] prefixes = new string[] {
            "http://*:80/",
        };
        Server server = new Server(prefixes, router);

        /* prevent console from exiting */
        Console.ReadLine();
    }
}
```
