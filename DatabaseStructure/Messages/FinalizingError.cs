using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class FinalizingError : Message
    {
        public Guid OrderId;
        public string ErrorMessage;
        
        public override MessageType MessageType => MessageType.FinalizingError;
    }
}