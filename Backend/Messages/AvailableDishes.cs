using System;
using System.Collections.Generic;

namespace Backend.Messages
{
    [Serializable]
    public class AvailableDishes : Message
    {
        public List<Dish> Dishes;

        public override MessageType MessageType => MessageType.AvailableDishes;
    }
}
