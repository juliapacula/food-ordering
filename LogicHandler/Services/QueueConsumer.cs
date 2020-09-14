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
            RabbitConfig rabbitConfig,
            IServiceProvider serviceProvider
        )
            : base(configuration, rabbitConfig)
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

            switch (msgType)
            {
                case MessageType.InitOrder:
                    await HandleInitOrderAsync(args, Message.Parse<InitOrder>(args.Body.ToArray()));
                    break;
                case MessageType.FinalizeOrder:
                    await HandleFinalizeOrderAsync(args, Message.Parse<FinalizeOrder>(args.Body.ToArray()));
                    break;
                case MessageType.CancelOrder:
                    await HandleCancelOrderAsync(args, Message.Parse<CancelOrder>(args.Body.ToArray()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task HandleInitOrderAsync(BasicDeliverEventArgs args, InitOrder initOrder)
        {
            using var scope = ServiceProvider.CreateScope();
            var command = scope.ServiceProvider.GetRequiredService<InitOrderCommand>();
            command.OrderId = initOrder.OrderId;

            try
            {
                await command.HandleAsync();
            }
            catch
            {
                PublishErrorMessage(initOrder.OrderId, args);
            }
        }

        private async Task HandleFinalizeOrderAsync(BasicDeliverEventArgs args, FinalizeOrder finalizeOrder)
        {
            using var scope = ServiceProvider.CreateScope();
            var command = scope.ServiceProvider.GetRequiredService<FinalizeOrderCommand>();
            command.Dishes = finalizeOrder.Dishes;
            command.Order = finalizeOrder.Order;

            var errors = command.Validate();
            Message finalMessage;

            if (errors.Count == 0)
            {
                var registrationMessage = new RegisterOrder
                {
                    OrderId = finalizeOrder.Order.Id,
                };
                var registrationMessageProps =
                    CreateProperties(args.BasicProperties.CorrelationId, MessageType.RegisterOrder);
                Channel.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, registrationMessageProps,
                    registrationMessage.GetSerialized());

                try
                {
                    await command.HandleAsync();
                }
                catch
                {
                    PublishErrorMessage(finalizeOrder.Order.Id, args);
                    return;
                }

                finalMessage = new FinalizingSuccess
                {
                    OrderId = finalizeOrder.Order.Id,
                    DeliveryDateTime = DateTime.Now.AddMinutes(new Random().Next(30, 120)),
                };
            }
            else
            {
                finalMessage = new FinalizingError
                {
                    OrderId = finalizeOrder.Order.Id,
                    ErrorMessage = errors.Values.ToList().First(),
                };
            }

            var replyProps = CreateProperties(args.BasicProperties.CorrelationId, finalMessage.MessageType);
            Channel.BasicPublish(string.Empty,
                args.BasicProperties.ReplyTo,
                replyProps,
                finalMessage.GetSerialized());
        }

        private async Task HandleCancelOrderAsync(BasicDeliverEventArgs args, CancelOrder cancelOrder)
        {
            using var scope = ServiceProvider.CreateScope();
            var command = scope.ServiceProvider.GetRequiredService<CancelOrderCommand>();
            command.OrderId = cancelOrder.OrderId;

            try
            {
                await command.HandleAsync();
            }
            catch
            {
                PublishErrorMessage(cancelOrder.OrderId, args);
            }
        }

        private IBasicProperties CreateProperties(string correlationId, MessageType type)
        {
            var properties = Channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>();
            properties.Headers.Add("Type", (int) type);
            properties.CorrelationId = correlationId;

            return properties;
        }

        private void PublishErrorMessage(Guid orderId, BasicDeliverEventArgs args)
        {
            var message = new Error()
            {
                OrderId = orderId,
                ErrorMessage = "Nie udało przetworzyć się zamówienia, problem z połączeniem"
            };
            var messageProps = CreateProperties(args.BasicProperties.CorrelationId, message.MessageType);
            Channel.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, messageProps, message.GetSerialized());
        }
    }
}