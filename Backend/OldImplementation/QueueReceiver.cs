using System;
using System.Linq;
using System.Text;
using Backend.Messages;
using DatabaseStructure;
using Microsoft.EntityFrameworkCore;
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
                case MessageType.FinalizeOrder:
                    success = HandleFinalizeOrderMessage(Message.Parse<FinalizeOrder>(args.Body));
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

        private bool HandleFinalizeOrderMessage(FinalizeOrder msg)
        {
            //var query = db.Dishes.Select(d => d.Price > 1);

            //Console.WriteLine(query);

            return true;
        }

        #endregion
    }
}
