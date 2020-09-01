using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Backend.QueueUtils
{
    public abstract class Queue
    {
        #region Fields

        protected readonly IConnection connection;
        protected readonly IModel channel;

        #endregion

        #region Constructor & Deconstructor

        protected Queue(string name, IConfiguration rabbitConf)
        {
            Name = name;

            var factory = new ConnectionFactory
            {
                UserName = rabbitConf["Username"],
                Password = rabbitConf["Password"],
                HostName = rabbitConf["ServerAddress"],
                VirtualHost = "/"
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(name, exclusive: false, autoDelete: false);
            channel.TxSelect();
            channel.BasicQos(0, 1, false);
        }

        ~Queue()
        {
            channel?.Dispose();
            connection?.Dispose();
        }

        #endregion

        #region Properties

        public string Name { get; }

        #endregion

        #region Methods

        protected IBasicProperties CreateBasicProperties(MessageType messageType, string correlationId, string replyQueueName = null)
        {
            var props = channel.CreateBasicProperties();
            
            if (replyQueueName != null)
                props.ReplyTo = replyQueueName;

            props.Headers = new Dictionary<string, object> { { "Type", (int)messageType } };
            props.CorrelationId = correlationId;

            return props;
        }

        #endregion
    }
}
