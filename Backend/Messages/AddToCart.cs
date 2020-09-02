using System;
using System.Collections.Generic;

namespace Backend.Messages
{
    [Serializable]
    public class AddToCart : Message
    {
        public Guid OrderId;
        public Dictionary<uint, uint> ProductsAndQuantity;

        public override MessageType MessageType => MessageType.AddToCart;
    }
}
