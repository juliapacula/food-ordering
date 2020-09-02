using System;

namespace Backend.Messages
{
    [Serializable]
    public class GetChosenDishes : Message
    {
        public Guid OrderId;

        public override MessageType MessageType => MessageType.GetChosenDishes;
    }
}
