using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class CancelOrder : Message
    {
        public Guid orderId;

        public override MessageType MessageType => MessageType.CancelOrder;
    }
}
