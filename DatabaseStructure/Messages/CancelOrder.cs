using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class CancelOrder : Message
    {
        public Guid OrderId { get; set; }

        public override MessageType MessageType => MessageType.CancelOrder;
    }
}