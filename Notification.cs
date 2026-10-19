using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationQueueTrigger
{
    public class Notification
    {
        private readonly ILogger<Notification> _logger;
        public Notification(ILogger<Notification> logger)
        {
            _logger = logger;
        }
        [Function(nameof(Notification))]
        public async Task Run([QueueTrigger("notifications", Connection = "QueueConnectionString")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
            var msg = System.Text.Json.JsonSerializer.Deserialize<QueueNotification>(message.MessageText);
            var email = msg?.email;
            var subject = msg?.subject;
            var body = msg?.body;
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(subject) || String.IsNullOrEmpty(body))
                return;
            try
            {
                await EmailService.SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
