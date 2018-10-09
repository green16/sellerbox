using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SellerBox.Common.Helpers
{
    public static class EmailHelper
    {
        public static Task SendEmail(string address, string text)
        {
            SmtpClient client = new SmtpClient("smtp.yandex.ru")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(Logins.EmailAddress, Logins.EmailPassword)
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(Logins.EmailAddress),
                Body = text,
                Subject = "Уведомление о событии"
            };
            mailMessage.To.Add(address);
            
            return client.SendMailAsync(mailMessage);
        }
    }
}
