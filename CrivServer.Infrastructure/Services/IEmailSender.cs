using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrivServer.Infrastructure.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);        
    }
    public interface ISmsSender
    {
        Task SendSmsAsync(string sms, string subject, string message);
    }
}
