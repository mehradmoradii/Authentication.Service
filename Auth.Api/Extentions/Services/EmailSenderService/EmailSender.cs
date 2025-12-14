using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace Auth.Api.Extentions.Services.EmailSenderService
{
    public class EmailSender : IEmailSenderService
    {

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration,
                           ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmail(string receptor, string subject, string body)
        {
            var email = _configuration.GetValue<string>("EMAIL_CONF:EMAIL");
            var password = _configuration.GetValue<string>("EMAIL_CONF:PASSWORD");
            var host = _configuration.GetValue<string>("EMAIL_CONF:HOST");
            var port = _configuration.GetValue<int>("EMAIL_CONF:PORT");

            var smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(email, password);
            var message = new MailMessage(email, receptor, subject, body);
            await smtpClient.SendMailAsync(message);
        }

        
    }
}
