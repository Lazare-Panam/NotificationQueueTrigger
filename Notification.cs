using Azure.Communication.Email;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure;
namespace NotificationQueueTrigger
{
    public class Notification
    {
        private readonly ILogger<Notification> _logger;
        private readonly EmailClient _emailClient;
        private readonly string _senderEmail;
        public Notification(ILogger<Notification> logger, EmailClient emailClient, IConfiguration configuration)
        {
            _logger = logger;
            _emailClient = emailClient;
            var sender = configuration["AcsSenderAddress"];
            ArgumentException.ThrowIfNullOrEmpty(sender);
            _senderEmail = sender;
        }
        [Function(nameof(Notification))]
        public async Task Run([QueueTrigger("notifications", Connection = "QueueConnectionString")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
            var msg = System.Text.Json.JsonSerializer.Deserialize<QueueNotification>(message.MessageText);
            var email = msg?.email;
            var subject = msg?.subject;
            var body = msg?.body;
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                _logger.LogWarning("Message {MessageId} missing email/subject/body; discarding.", message.MessageId);
                return;
            }
            try
            {
                await _emailClient.SendAsync(wait: WaitUntil.Completed, senderAddress: _senderEmail, recipientAddress: email, subject: subject, htmlContent: body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
    }
}
