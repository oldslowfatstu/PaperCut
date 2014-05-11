using System.Net.Mail;
using System.Threading.Tasks;

namespace Papercut.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.For(0, 1000, (int i) =>
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add("fred@test.com");
                mailMessage.Subject = "A test message " + i.ToString();
                mailMessage.Body = "The message body";
                mailMessage.IsBodyHtml = false;

                SmtpClient client = new SmtpClient();
                client.Send(mailMessage);
            });
        }
    }
}
