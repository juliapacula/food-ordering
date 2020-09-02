using System;

namespace Backend.Messages
{
    [Serializable]
    public class GetAvailableDishes : Message
    {
        public override MessageType MessageType => MessageType.GetAvailableDishes;
    }
}
