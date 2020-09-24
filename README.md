# SharpWebServer
Small wrapper for HttpListener in C# with fast routing

## Example
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
                request.Respond(random.Next(min, max).ToString());
            } else
            {
                // error
                request.Respond("Error: (min) must be smaller or equal to (max)");
            }
        }));

        /* instantiate new server */
        string[] prefixes = new string[] {
            "http://*:8888/",
        };
        Server server = new Server(prefixes, router);

        /* prevent console from exiting */
        Console.ReadLine();
    }
}
```
