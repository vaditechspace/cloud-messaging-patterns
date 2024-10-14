using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HRServiceTopicTrigger
{
    public class HRTopicTriggerFn
    {
        private readonly ILogger<HRTopicTriggerFn> _logger;
        private readonly string? _topicName = Environment.GetEnvironmentVariable("TopicName");
        private readonly string? _subscriptionName = Environment.GetEnvironmentVariable("SubscriptionName");

        public HRTopicTriggerFn(ILogger<HRTopicTriggerFn> logger)
        {
            _logger = logger;
        }

        [Function(nameof(HRTopicTriggerFn))]
        public async Task Run(
            [ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "SBConString")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body.ToString());
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                // Read the message body as a string
                var messageBody = message.Body.ToString();

                // HR specific code will process the message here. 

                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message with ID {id}", message.MessageId);

                // handle the failure, e.g. you might want to move to dead-letter queue.
                // await messageActions.DeadLetterMessageAsync(message);
            }
        }
    }
}
