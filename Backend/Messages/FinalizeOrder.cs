using System;

namespace Backend.Messages
{
    [Serializable]
    public class FinalizeOrder : Message
    {
        public Guid OrderId;
        public string Name;
        public string Surname;
        public string Email;

        public override MessageType MessageType => MessageType.FinalizeOrder;
    }
}
