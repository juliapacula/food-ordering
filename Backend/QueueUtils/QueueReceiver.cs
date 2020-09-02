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
            var type = GetMessageType(args);
            bool success;

            switch (type)
            {
                case MessageType.AddToCart:
                    success = HandleAddToCartMessage(Message.Parse<AddToCart>(args.Body));
                    break;
                
                // todo

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Reply(success, args.BasicProperties);
        }

        private void Reply(bool success, IBasicProperties originalMessageProperties)
        {
            var props = CreateBasicProperties(success ? MessageType.S_OK : MessageType.Error, originalMessageProperties.CorrelationId);
            var responseBytes = Encoding.UTF8.GetBytes("some response");

            channel.BasicPublish(string.Empty, originalMessageProperties.ReplyTo, props, responseBytes);
            channel.TxCommit();
        }

        #endregion

        #region Handlers

        private bool HandleAddToCartMessage(AddToCart msg)
        {
            Console.WriteLine(msg.OrderId);
            // todo
            return true;
        }

        #endregion
    }
}
