using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor) =>
            Options = optionsAccessor.Value;
        
        private AuthMessageSenderOptions Options { get; }
               
        public Task SendEmailAsync(string email, string subject, string message)
        {

            if (OptionsAvailable())
            {
                var client = new SmtpClient(Options.SenderClient)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Options.SenderUser, Options.SenderKey)
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(Options.DefaultSenderAddress)
                };
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = message;
                return client.SendMailAsync(mailMessage);                
            }
            else
            {
                WriteMessageToFile(email, subject, message);
            }
            return Task.CompletedTask;
        }
        public Task SendSms(string email, string subject, string message)
        {
            if (OptionsAvailable())
            {
                
            }
            else
            {
                WriteMessageToFile(email, subject, message);
            }
            return Task.CompletedTask;
        }

        private bool WriteMessageToFile(string email, string subject, string message)
        {
            var emailMessage = $"To: {email}\nSubject: {subject}\nMessage: {message}\n\n";
            var location = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "emails.txt").ToString();
            try
            {                
                if (!File.Exists(location))
                {
                    File.Create(location);
                }
                File.SetAttributes(location, FileAttributes.Normal);
                File.AppendAllText(location, emailMessage);
                
                return true;
            }
            catch { return false; }
            finally { File.SetAttributes(location, FileAttributes.ReadOnly); }
        }

        private bool OptionsAvailable() {
            if (Options != null && 
                !string.IsNullOrWhiteSpace(Options.DefaultSenderAddress) &&
                !string.IsNullOrWhiteSpace(Options.SenderClient) && 
                !string.IsNullOrWhiteSpace(Options.SenderUser) && 
                !string.IsNullOrWhiteSpace(Options.SenderKey))
                return true;
            return false;
        }
    }

    public class AuthMessageSenderOptions
    {
        public string SenderClient { get; set; }
        public string SenderUser { get; set; }
        public string SenderKey { get; set; }
        public string DefaultSenderAddress { get; set; }
    }

    public static class EmailSenderServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailSenderService(this IServiceCollection collection,
            Action<AuthMessageSenderOptions> setupAction)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            collection.Configure(setupAction);
            return collection.AddTransient<IEmailSender, EmailSender>();
        }
    }
}
