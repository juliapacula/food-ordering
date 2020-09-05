using System;
using System.Collections.Generic;
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
            var orderEntity = db.Orders.FirstOrDefault(order => order.Id == cancelOrder.orderId);
            db.Remove(orderEntity);
            db.SaveChanges();
        }

        private Message HandleFinalizeOrder(FinalizeOrder finalizeOrder)
        {
            var orderEntity = db.Orders.SingleOrDefault(order => order.Id == finalizeOrder.orderId) 
                              ?? new Order {Id = finalizeOrder.orderId};

            orderEntity.Address = finalizeOrder.address;
            orderEntity.Email = finalizeOrder.email;
            orderEntity.Name = finalizeOrder.name;
            orderEntity.Surname = finalizeOrder.surname;

            var errors = orderEntity.Validate(finalizeOrder);
            if (!string.IsNullOrEmpty(errors))
            {
                return new FinalizingError {orderId = finalizeOrder.orderId, errorMessage = errors};
            }

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
            var deliver = DateTime.Now.AddMinutes(new Random().Next(30, 120));
            return new FinalizingSuccess {orderId = finalizeOrder.orderId, deliveryDateTime = deliver};
        }

        private void HandleInitOrder(InitOrder initOrder)
        {
            db.Orders.Add(new Order {Id = initOrder.orderId});
            db.SaveChanges();
        }

        private AllDishes HandleGetAllDishes()
        {
            return new AllDishes {dishes = db.Dishes.ToList()};
        }

        #endregion
    }
}