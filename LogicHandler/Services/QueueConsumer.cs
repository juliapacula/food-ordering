using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure.Logic.Orders.Commands;
using DatabaseStructure.Messages;
using DatabaseStructure.QueueUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogicHandler.Services
{
    public class QueueConsumer : Queue
    {
        private IServiceProvider ServiceProvider { get; }
        private AsyncEventingBasicConsumer Consumer;

        public QueueConsumer(
            IConfiguration configuration,
            RabbitConfig _rabbitConfig,
            IServiceProvider serviceProvider
            )
            : base(configuration, _rabbitConfig)
        {
            ServiceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            Consumer = new AsyncEventingBasicConsumer(Channel);
            Consumer.Received += HandleMessageAsync;
            Channel.BasicConsume(RabbitConfig.QueueName, false, Consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs args)
        {
            Channel.BasicAck(args.DeliveryTag, false);
            var msgType = GetMessageType(args);
            Message replyMessage = null;

            switch (msgType)
            {
                case MessageType.InitOrder:
                    await HandleInitOrderAsync(Message.Parse<InitOrder>(args.Body.ToArray()));
                    break;
                case MessageType.FinalizeOrder:
                    replyMessage = await HandleFinalizeOrderAsync(Message.Parse<FinalizeOrder>(args.Body.ToArray()));
                    break;
                case MessageType.CancelOrder:
                    await HandleCancelOrderAsync(Message.Parse<CancelOrder>(args.Body.ToArray()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (replyMessage is null)
            {
                return;
            }

            var replyProps = CreateProperties(args.BasicProperties.CorrelationId, replyMessage.MessageType);
            Channel.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, replyProps, replyMessage.GetSerialized());
        }

        private async Task HandleInitOrderAsync(InitOrder initOrder)
        {
            using var scope = ServiceProvider.CreateScope();
            var command = scope.ServiceProvider.GetRequiredService<InitOrderCommand>();
            command.OrderId = initOrder.OrderId;

            await command.HandleAsync();
        }

        private async Task<Message> HandleFinalizeOrderAsync(FinalizeOrder finalizeOrder)
        {
            using var scope = ServiceProvider.CreateScope();
            var command = scope.ServiceProvider.GetRequiredService<FinalizeOrderCommand>();
            command.Dishes = finalizeOrder.Dishes;
            command.Order = finalizeOrder.Order;

            var errors = command.Validate();

            if (errors.Count == 0)
            {
                await command.HandleAsync();
            }
            else
            {
                return new FinalizingError()
                {
                    OrderId = finalizeOrder.Order.Id,
                    ErrorMessage = errors.Values.ToList().First(),
                };
            }

            var deliver = DateTime.Now.AddMinutes(new Random().Next(30, 120));

            return new FinalizingSuccess
            {
                OrderId = finalizeOrder.Order.Id,
                DeliveryDateTime = deliver,
            };
        }

        private async Task HandleCancelOrderAsync(CancelOrder cancelOrder)
        {
            using var scope = ServiceProvider.CreateScope();
            var command = scope.ServiceProvider.GetRequiredService<CancelOrderCommand>();
            command.OrderId = cancelOrder.OrderId;

            await command.HandleAsync();
        }

        private IBasicProperties CreateProperties(string correlationId, MessageType type)
        {
            var properties = Channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>();
            properties.Headers.Add("Type", (int) type);
            properties.CorrelationId = correlationId;

            return properties;
        }
    }
}