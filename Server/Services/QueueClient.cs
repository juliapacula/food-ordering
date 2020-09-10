using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure.Messages;
using DatabaseStructure.QueueUtils;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Server.Services
{
    public class QueueClient : Queue
    {
        public string ReplyQueueName { get; private set; }

        public QueueClient(IConfiguration configuration, RabbitConfig rabbitConfig)
            : base(configuration, rabbitConfig)
        {
        }

        public void Publish(Message msg)
        {
            var props = CreateProperties(Guid.NewGuid().ToString(), msg.MessageType);
            channel.BasicPublish(string.Empty, QueueName, props, msg.GetSerialized());
            channel.TxCommit();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            ReplyQueueName = channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReply;
            channel.BasicConsume(ReplyQueueName, true, consumer);

            return Task.CompletedTask;
        }

        private IBasicProperties CreateProperties(string correlationId, MessageType messageType)
        {
            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object> {{"Type", (int) messageType}};
            properties.CorrelationId = correlationId;
            properties.ReplyTo = ReplyQueueName;

            return properties;
        }

        private void OnReply(object sender, BasicDeliverEventArgs args)
        {
            // channel.BasicAck(args.DeliveryTag, false);
            channel.TxCommit();

            var msgType = GetMessageType(args);

            switch (msgType)
            {
                case MessageType.S_OK:
                case MessageType.TestCommand:
                    Console.WriteLine($"Command succesfully sent ({args.BasicProperties.CorrelationId})");
                    break;
                case MessageType.FinalizingError:
                    var errors = Message.Parse<FinalizingError>(args.Body.ToArray()).ErrorMessage;
                    // todo
                    break;
                case MessageType.FinalizingSuccess:
                    var deliveryTime = Message.Parse<FinalizingSuccess>(args.Body.ToArray()).DeliveryDateTime;
                    // todo
                    break;
                case MessageType.AllDishes:
                    var dishes = Message.Parse<AllDishes>(args.Body.ToArray()).dishes;
                    // todo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}