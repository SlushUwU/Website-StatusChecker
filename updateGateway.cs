using System.Net.Mail;

namespace GatewayPing
{
    public class updateGateway
    {
        
        public static void update(string currentGateway, string newGateway)
        {

            string from = "status@satowa-network.eu";
            string to = "";

            MailMessage message = new MailMessage(from, to);
            message.Subject = "Down";
            SmtpClient client = new SmtpClient("mail.satowa-network.eu");
            client.Credentials = new System.Net.NetworkCredential("status@satowa-network.eu", "");

            try
            {
                client.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"There was an issue sending the email: {e.ToString()}");
            }
        }
    }
}