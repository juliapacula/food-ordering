namespace Backend.QueueUtils
{
    public class RabbitConfig
    {
        public string QueueName { get; }

        public RabbitConfig(string _queueName)
        {
            QueueName = _queueName;
        }
    }
}