using System;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GatewayPing
{
    public class UpdateGateway
    {
        public static async Task Update(string currentGateway, string newGateway)
        {
            string from = "status@satowa-network.eu";
            string to = "it@satowa-network.eu";
            var gateways = new[]
            {
                new { Name = "Gateway Vienna 21", Host = "gateway-vie-21.satowa-network.dev"},
                new { Name = "Gateway Innsbruck 02", Host = "gateway-il-02.satowa-network.eu"},
                new { Name = "Gateway Frankfurt", Host = "gateway-f-100.satowa-network.eu" }
            };
            string zoneID = "b0ac2333c3d3b486e917ef3f54bea126";
            string dnsID = "908058f3832db6d80c2388c927fff232";
            string apiKey = "";

            if (from == gateways[0].Host)
            {
                // Erstellen Sie den PUT-Antrag
                var apiUrl = $"https://api.cloudflare.com/client/v4/zones/{zoneID}/dns_records/{dnsID}";
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestData = new
                {
                    content = newGateway,
                    data = new { },
                    name = "gateway.satowa-network.eu",
                    proxiable = true,
                    proxied = true,
                    ttl = 1,
                    type = "CNAME",
                    zone_id = zoneID,
                    zone_name = "satowa-network.eu",
                    tags = new string[] { }
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Gateway erfolgreich aktualisiert.");
                }
                else
                {
                    Console.WriteLine("Fehler beim Aktualisieren des Gateways: " + response.ReasonPhrase);
                }
            }
            else
            {
                MailMessage message = new MailMessage(from, to);
                message.Subject = "Down";
                SmtpClient client = new SmtpClient("mail.satowa-network.eu");
                client.Credentials = new System.Net.NetworkCredential("status@satowa-network.eu", "ieneSsANtaNC,16,;");
                try
                {
                    client.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Beim Senden der E-Mail ist ein Fehler aufgetreten: {e.ToString()}");
                }
            }
        }
    }
}
