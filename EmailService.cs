using Azure;
using Azure.Communication.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationQueueTrigger
{
    public class EmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderEmail;
        public EmailService(string connectionString, string senderEmail)
        {
            /*Todo : Wire up the conneciontstring properly*/
            _emailClient = new EmailClient(connectionString);
            _senderEmail = senderEmail;
        }
        public static async Task SendEmailAsync(string recipient, string subject, string htmlContent)
        {
            var obj = new EmailService(Environment.GetEnvironmentVariable("AcsConnectionString"), Environment.GetEnvironmentVariable("AcsSenderAddress"));
            var connectionString = Environment.GetEnvironmentVariable("AcsConnectionString");
            var sender = Environment.GetEnvironmentVariable("AcsSenderAddress");
            var response = await obj._emailClient.SendAsync(wait: WaitUntil.Completed, senderAddress: sender, recipientAddress: recipient, subject: subject, htmlContent: htmlContent);
        }
    }
}
