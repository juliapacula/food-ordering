using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class GetAllDishes : Message
    {
        public override MessageType MessageType => MessageType.GetAllDishes;
    }
}
