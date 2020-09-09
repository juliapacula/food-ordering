using System;
using System.Collections.Generic;
using DatabaseStructure.EntitySets;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class FinalizeOrder : Message
    {
        public Guid OrderId;
        public Order Order;
        public Dictionary<Guid, int> DishesAndQuantity;

        public override MessageType MessageType => MessageType.FinalizeOrder;
    }
}
