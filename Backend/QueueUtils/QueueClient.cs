using System;
using System.Collections.Generic;
using Backend.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Backend.QueueUtils
{
    public class QueueClient : Queue
    {
        #region Fields

        protected readonly string replyQueueName;

        #endregion

        #region Constructor

        public QueueClient(string name, IConfiguration rabbitConf) :
            base(name, rabbitConf)
        {
            replyQueueName = channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReply;
            channel.BasicConsume(replyQueueName, true, consumer);
        }

        #endregion

        #region Methods

        public void Publish(Message msg)
        {
            var props = CreateBasicProperties(msg.MessageType, Guid.NewGuid().ToString(), replyQueueName);

            channel.BasicPublish(string.Empty, Name, props, msg.GetSerialized());
            channel.TxCommit();
        }

        private void OnReply(object sender, BasicDeliverEventArgs args)
        {
            // reply
        }

        #endregion
    }
}
