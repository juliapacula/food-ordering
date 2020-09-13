using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class RegisterOrder : Message
    {
        public Guid OrderId { get; set; }

        public override MessageType MessageType => MessageType.RegisterOrder;
    }
}