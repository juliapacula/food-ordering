using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class InitOrder : Message
    {
        public Guid OrderId { get; set; }

        public override MessageType MessageType => MessageType.InitOrder;
    }
}