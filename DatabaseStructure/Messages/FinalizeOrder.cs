using System;
using System.Collections.Generic;
using DatabaseStructure.Logic.Orders.Models;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class FinalizeOrder : Message
    {
        public Order Order;
        public IEnumerable<Guid> Dishes;

        public override MessageType MessageType => MessageType.FinalizeOrder;
    }
}
