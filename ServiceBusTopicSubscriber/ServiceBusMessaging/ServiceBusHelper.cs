using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace ServiceBusMessaging
{
    public class ServiceBusHelper : IDisposable
    {
        private readonly string _connectionString;
        private ServiceBusClient _client;
        private ServiceBusSender? _sender;

        // Constructor: Initialize Service Bus client
        public ServiceBusHelper(string connectionString)
        {
            _connectionString = connectionString;
            _client = new ServiceBusClient(connectionString);
        }

        // Send a message to a queue or topic
        public async Task SendMessageAsync(string queueOrTopicName, string messageContent)
        {
            _sender = _client.CreateSender(queueOrTopicName);

            ServiceBusMessage message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageContent));

            try
            {
                // Send the message
                await _sender.SendMessageAsync(message);
                Console.WriteLine($"Message sent to {queueOrTopicName}: {messageContent}");
            }
            finally
            {
                await _sender.DisposeAsync();
            }
        }

        // Receive a message from a queue
        public async Task ReceiveMessageFromQueueAsync(string queueName, Func<string, Task> processMessage)
        {
            var receiver = _client.CreateReceiver(queueName);

            try
            {
                ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

                if (message != null)
                {
                    string messageBody = Encoding.UTF8.GetString(message.Body);

                    // Process the message
                    await processMessage(messageBody);

                    // Complete the message (remove it from the queue)
                    await receiver.CompleteMessageAsync(message);
                }
            }
            finally
            {
                await receiver.DisposeAsync();
            }
        }

        // Receive a message from a topic subscription
        public async Task ReceiveMessageFromSubscriptionAsync(string topicName, string subscriptionName, Func<string, Task> processMessage)
        {
            var receiver = _client.CreateReceiver(topicName, subscriptionName);

            try
            {
                ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

                if (message != null)
                {
                    string messageBody = Encoding.UTF8.GetString(message.Body);

                    // Process the message
                    await processMessage(messageBody);

                    // Complete the message (remove it from the subscription)
                    await receiver.CompleteMessageAsync(message);
                }
            }
            finally
            {
                await receiver.DisposeAsync();
            }
        }

        // Method to handle dead-lettered messages
        public async Task ReceiveDeadLetterMessageAsync(string queueOrTopicName, string subscriptionName = null)
        {
            var receiver = subscriptionName == null
                ? _client.CreateReceiver(queueOrTopicName, new ServiceBusReceiverOptions { SubQueue = SubQueue.DeadLetter })
                : _client.CreateReceiver(queueOrTopicName, subscriptionName, new ServiceBusReceiverOptions { SubQueue = SubQueue.DeadLetter });

            try
            {
                ServiceBusReceivedMessage deadLetterMessage = await receiver.ReceiveMessageAsync();

                if (deadLetterMessage != null)
                {
                    string messageBody = Encoding.UTF8.GetString(deadLetterMessage.Body);
                    Console.WriteLine($"Received dead-letter message: {messageBody}");

                    // Process the dead-lettered message, log or take action
                }
            }
            finally
            {
                await receiver.DisposeAsync();
            }
        }

        // Dispose method to clean up Service Bus client resources
        public void Dispose()
        {
            if (_client != null)
            {
                _client.DisposeAsync().GetAwaiter().GetResult();
                _client = null;
            }
        }
    }
}
