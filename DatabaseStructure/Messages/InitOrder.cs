using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class InitOrder : Message
    {
        public Guid orderId;

        public override MessageType MessageType => MessageType.InitOrder;
    }
}
