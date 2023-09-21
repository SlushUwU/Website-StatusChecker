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
        /// Down = GAteway ${Gateway} is down
        /// </returns>
        static async Task Main(string[] args)
        {
            // Create a ping test and create a statusCheck and wait for its reply. If it returns true Gateway Vienna 21 is up.
            if (pingTest("gateway-vie-21.satowa-network.eu") && await statusCheck("https://gateway-vie-21.satowa-network.eu/"))
            {
                Console.WriteLine("Gateway Vienna 21 is Up...");
            }
            // If Gateway Vienna 21 is not reachable or gives an unknown error back then try to switch the Gateway to Gateway il 02
            else if (pingTest("gateway-il-02.satowa-network.eu") && await statusCheck("https://gateway-il-02.satowa-network.eu/"))
            {
                Console.WriteLine("Gateway Innsbruck 02 is Up");
            }
            // If Gateway IL 21 is also not up try to use Gateway Frankfurt 100
            else if (pingTest("gateway-f-100.satowa-network.eu") && await statusCheck("https://gateway-f-100.satowa-network.eu/"))
            {
                Console.WriteLine("Gateway Frankfurt is Up...");
            }
            // If nothing is up everything is down and nothing can be routed 
            else
            {
                Console.WriteLine("All hosts are down!");
            }

            // Wait for ~5 Minutes i guess and...
            Thread.Sleep(300000);
            // execute Main again
            await Main(null);
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
                // If the response is a 2xx response then return true
                if (response.IsSuccessStatusCode)
                {
                    
                    return true;
                }
                // If it is something else then return false
                else
                {
                    Console.Write(response);
                    return false;
                }
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
            // If it was successfully then return true
            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            // If not return false
            else
            {
                return false;
            }
        }
    }
}

