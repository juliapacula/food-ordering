using System;
using DatabaseStructure.Logic.Dishes.Models;
using DatabaseStructure.Logic.Orders.Models;

namespace DatabaseStructure.EntitySets
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