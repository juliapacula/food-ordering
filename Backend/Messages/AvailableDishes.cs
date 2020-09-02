using System;
using System.Collections.Generic;
using DatabaseStructure;

namespace Backend.Messages
{
    [Serializable]
    public class AvailableDishes : Message
    {
        public List<Dish> Dishes;

        public override MessageType MessageType => MessageType.AvailableDishes;
    }
}
