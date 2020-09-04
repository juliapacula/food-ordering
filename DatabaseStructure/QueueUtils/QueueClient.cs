using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace DatabaseStructure.QueueUtils
{
    public class QueueClient : Queue
    {
        #region Constructor

        public QueueClient(IConfiguration _appSettings, RabbitConfig _rabbitConfig)
            : base(_appSettings, _rabbitConfig)
        {
        }

        #endregion

        #region Methods

        public void Publish(Message msg)
        {
            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object> { { "Type", (int)msg.MessageType } };

            channel.BasicPublish(string.Empty, QueueName, props, msg.GetSerialized());
            channel.TxCommit();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        #endregion
    }
}
