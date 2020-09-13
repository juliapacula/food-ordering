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
        private string ReplyQueueName { get; set; }
        private Guid CorrelationId { get; set; }
        private AsyncEventingBasicConsumer Consumer { get; set; }

        public QueueClient(IConfiguration configuration, RabbitConfig rabbitConfig)
            : base(configuration, rabbitConfig)
        {
        }

        public void Publish(Message msg)
        {
            var properties = CreateProperties();
            properties.Headers.Add("Type", (int) msg.MessageType);
            Channel.BasicPublish(string.Empty, QueueName, properties, msg.GetSerialized());
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            ReplyQueueName = Channel.QueueDeclare().QueueName;
            Consumer = new AsyncEventingBasicConsumer(Channel);
            Consumer.Received += OnReply;
            Channel.BasicConsume(ReplyQueueName, true, Consumer);
            CorrelationId = Guid.NewGuid();

            return Task.CompletedTask;
        }

        private IBasicProperties CreateProperties()
        {
            var properties = Channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>();
            properties.CorrelationId = CorrelationId.ToString();
            properties.ReplyTo = ReplyQueueName;

            return properties;
        }

        private Task OnReply(object sender, BasicDeliverEventArgs args)
        {
            if (args.BasicProperties.CorrelationId != CorrelationId.ToString())
            {
                return Task.CompletedTask;
            }

            var msgType = GetMessageType(args);

            switch (msgType)
            {
                case MessageType.FinalizingError:
                    var errors = Message.Parse<FinalizingError>(args.Body.ToArray()).ErrorMessage;
                    // todo
                    break;
                case MessageType.FinalizingSuccess:
                    var deliveryTime = Message.Parse<FinalizingSuccess>(args.Body.ToArray()).DeliveryDateTime;
                    // todo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }
    }
}