using DatabaseStructure.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DatabaseStructure.QueueUtils
{
    public abstract class Queue : BackgroundService
    {
        #region Fields & Properties

        protected readonly RabbitConfig rabbitConfig;
        protected IConnection connection;
        protected IModel channel;

        public string QueueName => rabbitConfig.QueueName;

        #endregion

        #region Constructor

        protected Queue(IConfiguration _appSettings, RabbitConfig _rabbitConfig)
        {
            rabbitConfig = _rabbitConfig;
            InitRabbitMQ(_appSettings.GetSection("RabbitMq"));
        }

        #endregion

        #region Overrides

        public override void Dispose()
        {
            channel?.Close();
            connection?.Close();
            base.Dispose();
        }

        #endregion

        #region Methods

        private void InitRabbitMQ(IConfiguration rabbitAppSettings)
        {
            var factory = new ConnectionFactory
            {
                UserName = rabbitAppSettings["Username"],
                Password = rabbitAppSettings["Password"],
                HostName = rabbitAppSettings["ServerAddress"],
                VirtualHost = "/"
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(QueueName, exclusive: false, autoDelete: false);
            channel.TxSelect();
            channel.BasicQos(0, 1, false);
        }

        protected MessageType GetMessageType(BasicDeliverEventArgs args)
        {
            return args.BasicProperties.Headers.TryGetValue("Type", out var obj) ? (MessageType)obj : default;
        }

        #endregion
    }
}