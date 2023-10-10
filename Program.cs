using System.Net.NetworkInformation;
using System.Text;



namespace GatewayPing
{
    class Project
    {
        
        /// <summary>
        /// Main class that includes, routes and provides all important features and connection
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// Up = Gateway ${Gateway} is up
        /// Down = Gateway ${Gateway} is down
        /// </returns>
        static async Task Main(string[] args)
        {
            
            // Define the gateway information
            var gateways = new[]
            {
                new { Name = "Gateway Vienna 21", Host = "gateway-vie-21.satowa-network.eu" },
                new { Name = "Gateway Innsbruck 02", Host = "gateway-il-02.satowa-network.eu" },
                new { Name = "Gateway Frankfurt", Host = "gateway-f-100.satowa-network.eu" }
            };
            int hostCount = 0;
            foreach (var gateway in gateways)
            {
                if (pingTest(gateway.Host) && await statusCheck($"https://{gateway.Host}/"))
                {
                    Console.WriteLine($"{gateway.Name} is Up...");
                    hostCount++;
                    Thread.Sleep(300000); // Wait for ~5 minutes
                    continue; // Move on to the next gateway
                }
            }

            // If none of the gateways are up, display a message
            Console.WriteLine("All hosts are down!");
        }

        /// <summary>
        /// Get the headers from the given host, use a Secret UserAgent to bypass Bad Bots WAF Rule and return a bool. If up true, if down false
        /// </summary>
        /// <param name="host">https://google.com</param>
        /// <returns>bool</returns>
        static async Task<bool> statusCheck(string host)
        {
            // Use the HttpClient() Lib provided by .NET Core, and create a new HttpCLient called httpClient
            using (var httpClient = new HttpClient())
            {
                // Set the BaseAddress with a new Uri to get a few issues fixed
                httpClient.BaseAddress = new Uri(host);
                // USe the Secret User Agent to bypass the Bot Fight MOde on Cloudflare WAF
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("");
                // Send the Data and fetch the Response async
                HttpResponseMessage response = await httpClient.GetAsync(host);
                // If the response is not 2xx response then return false
                if (!response.IsSuccessStatusCode)
                {
                    Console.Write(response);
                    updateGateway.update(host, "NaN");
                }
                return true;
            }
        }
        /// <summary>
        ///     Probably a very useless class but just to learn something lol
        /// </summary>
        /// <param name="host"></param>
        /// <returns>bool</returns>
        private static bool pingTest(string host)
        {
            // Use the Ping lib from .NET Core, and create a new Ping called pingSender
            Ping pingSender = new Ping();
            // Create a new PingOption also called options
            PingOptions options = new PingOptions();

            // Set the options to DontFragment to true 
            options.DontFragment = true;

            // Send a package with the text "Ping"
            string data = "Ping";
            // Trasnlate it into a Byte buffer
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            // Provide the timeout
            int timeout = 5000;
            // Send the Ping
            PingReply reply = pingSender.Send(host, timeout, buffer, options);
            // If it was not successful then return false
            if (reply.Status != IPStatus.Success) return false;

            return true;
        }
    }
}

