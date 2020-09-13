using System;
using DatabaseStructure.Logic.Dishes.Models;

namespace DatabaseStructure.Logic.Orders.Models
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