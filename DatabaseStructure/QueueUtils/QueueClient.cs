using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DatabaseStructure.QueueUtils
{
    public class QueueClient : Queue
    {
        #region MyRegion

        public string ReplyQueueName { get; private set; }

        #endregion

        #region Constructor

        public QueueClient(IConfiguration _appSettings, RabbitConfig _rabbitConfig)
            : base(_appSettings, _rabbitConfig)
        {
        }

        #endregion

        #region Methods

        public void Publish(Message msg)
        {
            var props = CreateProperties(Guid.NewGuid().ToString(), msg.MessageType);

            channel.BasicPublish(string.Empty, QueueName, props, msg.GetSerialized());
            channel.TxCommit();
        }

        protected IBasicProperties CreateProperties(string correlationId, MessageType messageType)
        {
            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object> { { "Type", (int)messageType } };
            properties.CorrelationId = correlationId;
            properties.ReplyTo = ReplyQueueName;
            return properties;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            ReplyQueueName = channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReply;
            channel.BasicConsume(ReplyQueueName, true, consumer);

            return Task.CompletedTask;
        }

        private void OnReply(object sender, BasicDeliverEventArgs args)
        {
            channel.BasicAck(args.DeliveryTag, false);
            channel.TxCommit();

            var msgType = GetMessageType(args);

            switch (msgType)
            {
                case MessageType.S_OK:
                    Console.WriteLine($"Command succesfully sent ({args.BasicProperties.CorrelationId})");
                    break;
                case MessageType.FinalizingError:
                    var errors = Message.Parse<FinalizingError>(args.Body).errorMessage;
                    // todo
                    break;
                case MessageType.FinalizingSuccess:
                    // todo
                    break;
                case MessageType.AllDishes:
                    var dishes = Message.Parse<AllDishes>(args.Body).dishes;
                    // todo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
