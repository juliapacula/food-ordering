using System;

namespace DatabaseStructure
{
    public class DishInOrder
    {
        public Guid DishId { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
    }
}