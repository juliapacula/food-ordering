using System;
using System.Threading;
using DatabaseStructure.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace DatabaseStructure.QueueUtils
{
    public abstract class Queue : BackgroundService
    {
        protected readonly RabbitConfig RabbitConfig;
        protected IModel Channel;
        protected string QueueName => RabbitConfig.QueueName;
        private IConnection Connection;

        protected Queue(IConfiguration configuration, RabbitConfig rabbitConfig)
        {
            RabbitConfig = rabbitConfig;
            InitRabbitMq(configuration.GetSection("RabbitMq"));
        }

        public override void Dispose()
        {
            Channel?.Close();
            Connection?.Close();
            base.Dispose();
        }

        private void InitRabbitMq(IConfiguration configuration)
        {
            try
            {
                CreateConnection(configuration);
            }
            catch (BrokerUnreachableException _)
            {
                Thread.Sleep(10000);
                CreateConnection(configuration);
            }

            Channel.QueueDeclare(QueueName, exclusive: false, autoDelete: false);
            Channel.BasicQos(0, 1, false);
        }

        protected MessageType GetMessageType(BasicDeliverEventArgs args)
        {
            return args.BasicProperties.Headers.TryGetValue("Type", out var obj) ? (MessageType) obj : default;
        }

        private void CreateConnection(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                UserName = configuration["Username"],
                Password = configuration["Password"],
                HostName = configuration["ServerAddress"],
                DispatchConsumersAsync = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
            };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
        }
    }
}