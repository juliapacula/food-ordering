using System;
using System.ComponentModel.DataAnnotations;

namespace DatabaseStructure
{
    public class DishInOrder
    {
        public Guid DishId { get; set; }
        public Dish Dish { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
    }
}