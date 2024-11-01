using System.Net.Mail;
using System.Net;

namespace ServicesForShelfSwap.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string otp)
        {
            var smtpHost = _configuration["SMTP:Host"];
            var smtpPort = int.Parse(_configuration["SMTP:Port"]);
            var enableSSL = bool.Parse(_configuration["SMTP:EnableSSL"]);
            var userName = _configuration["SMTP:UserName"];
            var password = _configuration["SMTP:Password"];
            var fromEmail = _configuration["SMTP:FromEmail"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(userName, password);
                client.EnableSsl = enableSSL;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Your OTP Code",
                    Body = $"Your OTP code is: {otp}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(toEmail);

                client.Send(mailMessage);
            }
        }
    }
}
