using MailKit.Net.Smtp;
using MimeKit;
using MailKit;
using System.Threading.Tasks;
namespace QontrolSystem.Services

{
    public class ServiceEmail
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public ServiceEmail()
        {
            // Ideally, read these from appsettings.json or secrets
            _smtpServer = "smtp.gmail.com";
            _smtpPort = 587;
            _smtpUser = "alsonkhanyile69@gmail.com";      // your email address
            _smtpPass = "nmpt lvyq fnof pxil";                // your app password
            _senderEmail = "alsonkhanyile69@gmail.com";   // same as user
            _senderName = "Qontrol System";
        }

        public async Task SendEmailAsync(string recipientEmail, string recipientName, string subject, string htmlMessage)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_senderName, _senderEmail));
            email.To.Add(new MailboxAddress(recipientName, recipientEmail));

            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpUser, _smtpPass);

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
