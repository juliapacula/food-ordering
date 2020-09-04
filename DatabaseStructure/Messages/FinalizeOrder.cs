using System;
using System.Collections.Generic;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class FinalizeOrder : Message
    {
        public Guid orderId;
        public string name;
        public string surname;
        public string email;
        public string address;
        public Dictionary<Guid, int> dishesAndQuantity;

        public override MessageType MessageType => MessageType.FinalizeOrder;
    }
}
