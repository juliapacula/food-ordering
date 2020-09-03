using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Backend.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Backend.QueueUtils
{
    public class QueueHandler : BackgroundService
    {
        #region Fields

        private readonly string queueName;
        private IConnection connection;
        private IModel channel;

        #endregion

        #region Constructor

        public QueueHandler(IConfiguration configuration)
        {
            queueName = "food_ordering";
            InitRabbitMQ(configuration.GetSection("RabbitMq"));
        }

        #endregion

        #region Overrides

        public override void Dispose()
        {
            channel?.Close();
            connection?.Close();
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += HandleMessage;
            channel.BasicConsume(queueName, false, consumer);

            return Task.CompletedTask;
        }

        #endregion

        #region Methods

        private void InitRabbitMQ(IConfiguration rabbitConf)
        {
            var factory = new ConnectionFactory
            {
                UserName = rabbitConf["Username"],
                Password = rabbitConf["Password"],
                HostName = rabbitConf["ServerAddress"],
                VirtualHost = "/"
            };

            // create connection  
            connection = factory.CreateConnection();

            // create channel  
            channel = connection.CreateModel();

            channel.QueueDeclare(queueName, exclusive: false, autoDelete: false);
            channel.TxSelect();
            channel.BasicQos(0, 1, false);
        }


        private void HandleMessage(object sender, BasicDeliverEventArgs args)
        {
            var msgType = GetMessageType(args);

            switch (msgType)
            {
                case MessageType.AddToCart:
                    Console.WriteLine(Message.Parse<AddToCart>(args.Body).OrderId);
                    break;
            }

            channel.BasicAck(args.DeliveryTag, false);
            channel.TxCommit();
        }

        private MessageType GetMessageType(BasicDeliverEventArgs args)
        {
            return args.BasicProperties.Headers.TryGetValue("Type", out var obj) ? (MessageType)obj : default;
        }

        #endregion
    }
}