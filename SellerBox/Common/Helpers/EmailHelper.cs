using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SellerBox.Common.Helpers
{
    public static class EmailHelper
    {
        public static Task SendEmail(string smtpUser, string smtpPassword, string fromAddress, string toAddress, string text)
        {
            SmtpClient client = new SmtpClient("smtp.yandex.ru")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPassword)
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(fromAddress),
                Body = text,
                Subject = "Уведомление о событии"
            };
            mailMessage.To.Add(toAddress);
            
            return client.SendMailAsync(mailMessage);
        }
    }
}
