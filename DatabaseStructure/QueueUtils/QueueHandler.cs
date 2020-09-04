using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure.EntitySets;
using DatabaseStructure.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DatabaseStructure.QueueUtils
{
    public class QueueHandler : Queue
    {
        #region Fields

        private readonly DatabaseContext db;

        #endregion

        #region Constructor

        public QueueHandler(IConfiguration _appSettings, RabbitConfig _rabbitConfig, DatabaseContext _db)
            : base(_appSettings, _rabbitConfig)
        {
            db = _db;
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
            var msgType = GetMessageType(args);

            switch (msgType)
            {
                case MessageType.InitOrder:
                    HandleInitOrder(Message.Parse<InitOrder>(args.Body));
                    break;
                case MessageType.FinalizeOrder:
                    HandleFinalizeOrder(Message.Parse<FinalizeOrder>(args.Body));
                    break;
                case MessageType.CancelOrder:
                    HandleCancelOrder(Message.Parse<CancelOrder>(args.Body));
                    break;
                case MessageType.TestCommand:
                    Console.WriteLine(Message.Parse<TestCommand>(args.Body).testMessage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            channel.BasicAck(args.DeliveryTag, false);
            channel.TxCommit();
        }

        private MessageType GetMessageType(BasicDeliverEventArgs args)
        {
            return args.BasicProperties.Headers.TryGetValue("Type", out var obj) ? (MessageType)obj : default;
        }

        #endregion

        #region Handlers

        private void HandleCancelOrder(CancelOrder cancelOrder)
        {
            var orderEntity = db.Orders.FirstOrDefault(order => order.Id == cancelOrder.orderId);
            db.Remove(orderEntity);
            db.SaveChanges();
        }

        private void HandleFinalizeOrder(FinalizeOrder finalizeOrder)
        {
            var orderEntity = db.Orders.Single(order => order.Id == finalizeOrder.orderId);

            orderEntity.Address = finalizeOrder.address;
            orderEntity.Email = finalizeOrder.email;
            orderEntity.Name = finalizeOrder.name;
            orderEntity.Surname = finalizeOrder.surname;

            db.Update(orderEntity);

            foreach (var (guid, quantity) in finalizeOrder.dishesAndQuantity)
            {
                var dish = db.Dishes.Single(d => d.Id == guid);

                var dishEntity = new DishInOrder
                {
                    Dish = dish,
                    DishId = dish.Id,
                    Order = orderEntity,
                    OrderId = orderEntity.Id,
                    Quantity = quantity,
                };

                db.DishInOrders.Add(dishEntity);
            }

            db.SaveChanges();
        }

        private void HandleInitOrder(InitOrder initOrder)
        {
            db.Orders.Add(new Order { Id = initOrder.orderId });
            db.SaveChanges();
        }

        #endregion
    }
}