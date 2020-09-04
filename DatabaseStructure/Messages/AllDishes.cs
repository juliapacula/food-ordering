using System;
using System.Collections.Generic;
using DatabaseStructure.EntitySets;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class AllDishes : Message
    {
        public List<Dish> dishes;

        public override MessageType MessageType => MessageType.AllDishes;
    }
}