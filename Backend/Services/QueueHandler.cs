using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure;
using DatabaseStructure.EntitySets;
using DatabaseStructure.Messages;
using DatabaseStructure.QueueUtils;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Backend.Services
{
    public class QueueHandler : Queue
    {
        #region Fields

        private readonly DatabaseContext _context;

        #endregion

        #region Constructor

        public QueueHandler(IConfiguration configuration, RabbitConfig _rabbitConfig, DatabaseContext context)
            : base(configuration, _rabbitConfig)
        {
            _context = context;
        }

        #endregion

        #region Methods

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += HandleMessage;
            channel.BasicConsume(rabbitConfig.QueueName, false, consumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(object sender, BasicDeliverEventArgs args)
        {
            channel.BasicAck(args.DeliveryTag, false);

            var msgType = GetMessageType(args);
            Message replyMessage = null;

            switch (msgType)
            {
                case MessageType.InitOrder:
                    HandleInitOrder(Message.Parse<InitOrder>(args.Body));
                    break;
                case MessageType.FinalizeOrder:
                    replyMessage = HandleFinalizeOrder(Message.Parse<FinalizeOrder>(args.Body));
                    break;
                case MessageType.CancelOrder:
                    HandleCancelOrder(Message.Parse<CancelOrder>(args.Body));
                    break;
                case MessageType.TestCommand:
                    Console.WriteLine(Message.Parse<TestCommand>(args.Body).testMessage);
                    break;
                case MessageType.GetAllDishes:
                    replyMessage = HandleGetAllDishes();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            replyMessage ??= new Success();

            var replyProps = channel.CreateBasicProperties();
            replyProps.Headers = new Dictionary<string, object> {{"Type", (int) replyMessage.MessageType}};
            replyProps.CorrelationId = args.BasicProperties.CorrelationId;

            channel.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, replyProps, replyMessage.GetSerialized());
            channel.TxCommit();
        }

        #endregion

        #region Handlers

        private void HandleCancelOrder(CancelOrder cancelOrder)
        {
            var orderEntity = _context.Orders.FirstOrDefault(order => order.Id == cancelOrder.orderId);
            _context.Remove(orderEntity);
            _context.SaveChanges();
        }

        private Message HandleFinalizeOrder(FinalizeOrder finalizeOrder)
        {
            var orderEntity = _context.Orders.SingleOrDefault(order => order.Id == finalizeOrder.Order.Id)
                              ?? finalizeOrder.Order;

            var errors = orderEntity.Validate(finalizeOrder);

            if (!string.IsNullOrEmpty(errors))
            {
                return new FinalizingError {OrderId = finalizeOrder.Order.Id, ErrorMessage = errors};
            }

            _context.Update(orderEntity);
            _context.SaveChanges();

            foreach (var (guid, quantity) in finalizeOrder.DishesAndQuantity)
            {
                var dish = _context.Dishes.Single(d => d.Id == guid);

                var dishEntity = new DishInOrder
                {
                    Dish = dish,
                    DishId = dish.Id,
                    Order = orderEntity,
                    OrderId = orderEntity.Id,
                    Quantity = quantity,
                };

                _context.DishInOrders.Add(dishEntity);
            }

            _context.SaveChanges();

            var deliver = DateTime.Now.AddMinutes(new Random().Next(30, 120));
            
            return new FinalizingSuccess { OrderId = finalizeOrder.Order.Id, DeliveryDateTime = deliver };
        }

        private void HandleInitOrder(InitOrder initOrder)
        {
            _context.Orders.Add(new Order {Id = initOrder.OrderId});
            _context.SaveChanges();
        }

        private AllDishes HandleGetAllDishes()
        {
            return new AllDishes {dishes = _context.Dishes.ToList()};
        }

        #endregion
    }
}