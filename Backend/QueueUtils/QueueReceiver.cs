using System;
using System.Text;
using Backend.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Backend.QueueUtils
{
    public class QueueReceiver : Queue
    {
        #region Constructor

        public QueueReceiver(string name, IConfiguration rabbitConfiguration) 
            : base(name, rabbitConfiguration)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReceiveMessage;
            channel.BasicConsume(name, true, consumer);
        }

        #endregion

        #region Methods

        private void ReceiveMessage(object model, BasicDeliverEventArgs args)
        {
            var type = args.BasicProperties.Headers.TryGetValue("Type", out var obj) ? (MessageType)obj : default;
            var success = false;

            switch (type)
            {
                case MessageType.AddToCart:
                    success = HandleAddToCartMessage(Message.Parse<AddToCart>(args.Body));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var props = CreateBasicProperties(success ? MessageType.S_OK : MessageType.Error, args.BasicProperties.CorrelationId);
            var responseBytes = Encoding.UTF8.GetBytes("some response");

            channel.BasicPublish(string.Empty, args.BasicProperties.ReplyTo, props, responseBytes);
            channel.TxCommit();
        }

        private bool HandleAddToCartMessage(AddToCart msg)
        {
            Console.WriteLine(msg.OrderId);
            // todo
            return true;
        }

        #endregion
    }
}
