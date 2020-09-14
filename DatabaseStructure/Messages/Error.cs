using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class Error : Message
    {
        public Guid OrderId;
        public string ErrorMessage;
        
        public override MessageType MessageType => MessageType.Error;
    }
}