using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ITServiceBusTopicTrigger
{
    public class ITTopicTriggerFn
    {
        private readonly ILogger<ITTopicTriggerFn> _logger;
        private readonly string? _topicName = Environment.GetEnvironmentVariable("TopicName");
        private readonly string? _subscriptionName = Environment.GetEnvironmentVariable("SubscriptionName");


        public ITTopicTriggerFn(ILogger<ITTopicTriggerFn> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ITTopicTriggerFn))]
        public async Task Run(
            [ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "SBConString")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // IT specific code will process the message here. 

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
