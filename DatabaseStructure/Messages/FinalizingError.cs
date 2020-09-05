using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class FinalizingError : Message
    {
        public Guid orderId;
        public string errorMessage;
        
        public override MessageType MessageType => MessageType.FinalizingError;
    }
}