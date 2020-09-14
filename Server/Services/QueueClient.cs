using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure.Messages;
using DatabaseStructure.QueueUtils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Server.Hubs;

namespace Server.Services
{
    public class QueueClient : Queue
    {
        private readonly IHubContext<OrderFulfillmentHub> _hubContext;
        private string ReplyQueueName { get; set; }
        private Guid CorrelationId { get; set; }
        private AsyncEventingBasicConsumer Consumer { get; set; }

        public QueueClient(IConfiguration configuration, RabbitConfig rabbitConfig,
            IHubContext<OrderFulfillmentHub> hubContext)
            : base(configuration, rabbitConfig)
        {
            _hubContext = hubContext;
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
                case MessageType.RegisterOrder:
                    var message = Message.Parse<RegisterOrder>(args.Body.ToArray());
                    _hubContext.Clients.Group(message.OrderId.ToString())
                        .SendAsync(OrderFulfillmentMessages.Registered);
                    break;
                case MessageType.FinalizingError:
                    var error = Message.Parse<FinalizingError>(args.Body.ToArray());
                    _hubContext.Clients.Group(error.OrderId.ToString()).SendAsync(
                        OrderFulfillmentMessages.Failed,
                        error.ErrorMessage);
                    break;
                case MessageType.FinalizingSuccess:
                    var success = Message.Parse<FinalizingSuccess>(args.Body.ToArray());
                    _hubContext.Clients.Group(success.OrderId.ToString()).SendAsync(
                        OrderFulfillmentMessages.Completed,
                        success.DeliveryDateTime);
                    break;
                case MessageType.Error:
                    var defaultError = Message.Parse<Error>(args.Body.ToArray());
                    _hubContext.Clients.Group(defaultError.OrderId.ToString()).SendAsync(
                        OrderFulfillmentMessages.Error,
                        defaultError.ErrorMessage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }
    }
}