using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class FinalizingSuccess : Message
    {
        public Guid orderId;
        public DateTime deliveryDateTime;
        
        public override MessageType MessageType => MessageType.FinalizingSuccess;
    }
}