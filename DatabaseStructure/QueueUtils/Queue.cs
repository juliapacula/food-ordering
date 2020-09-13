using DatabaseStructure.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DatabaseStructure.QueueUtils
{
    public abstract class Queue : BackgroundService
    {
        protected readonly RabbitConfig RabbitConfig;
        protected IConnection Connection;
        protected IModel Channel;

        public string QueueName => RabbitConfig.QueueName;

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
            var factory = new ConnectionFactory
            {
                UserName = configuration["Username"],
                Password = configuration["Password"],
                HostName = configuration["ServerAddress"],
                DispatchConsumersAsync = true,
            };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();

            Channel.QueueDeclare(QueueName, exclusive: false, autoDelete: false);
            // Channel.TxSelect();
            Channel.BasicQos(0, 1, false);
        }

        protected MessageType GetMessageType(BasicDeliverEventArgs args)
        {
            return args.BasicProperties.Headers.TryGetValue("Type", out var obj) ? (MessageType) obj : default;
        }
    }
}