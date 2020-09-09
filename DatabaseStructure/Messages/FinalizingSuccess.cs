using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class FinalizingSuccess : Message
    {
        public Guid OrderId;
        public DateTime DeliveryDateTime;
        
        public override MessageType MessageType => MessageType.FinalizingSuccess;
    }
}