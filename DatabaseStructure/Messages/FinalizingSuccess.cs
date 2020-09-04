using System;

namespace DatabaseStructure.Messages
{
    public class FinalizingSuccess : Message
    {
        public Guid orderId;
        
        public override MessageType MessageType => MessageType.FinalizingSuccess;
    }
}