using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = $"To: {email}\nSubject: {subject}\nMessage: {message}\n\n";
            var location = string.Format(@"{0}\emails.txt", AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
            if(!File.Exists(location)){
                File.Create(location);             
            }
            File.SetAttributes(location, FileAttributes.Normal);
            File.AppendAllText(location, emailMessage);
            File.SetAttributes(location, FileAttributes.ReadOnly);
            return Task.CompletedTask;
        }
        public Task SendSmsAsync(string email, string subject, string message)
        {
            var emailMessage = $"To: {email}\nSubject: {subject}\nMessage: {message}\n\n";
            var location = string.Format(@"{0}\emails.txt", AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
            if (!File.Exists(location))
            {
                File.Create(location);
            }
            File.SetAttributes(location, FileAttributes.Normal);
            File.AppendAllText(location, emailMessage);
            File.SetAttributes(location, FileAttributes.ReadOnly);
            return Task.CompletedTask;
        }
    }

    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}
