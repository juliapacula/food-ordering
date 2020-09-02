using System;

namespace Backend.Messages
{
    [Serializable]
    public class FinalizeOrder : Message
    {
        public Guid OrderId;

        public override MessageType MessageType => MessageType.FinalizeOrder;
    }
}
