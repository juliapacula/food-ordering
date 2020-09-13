using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure.Logic.Orders.Models;
using DatabaseStructure.Logic.Shared;

namespace DatabaseStructure.Logic.Orders.Commands
{
    public class FinalizeOrderCommand : ICommand
    {
        public Order Order;
        public IEnumerable<Guid> Dishes;
        private DatabaseContext _context;

        public FinalizeOrderCommand(DatabaseContext context)
        {
            _context = context;
        }

        public async Task HandleAsync()
        {
            var dishesInOrder = GetDishesAndQuantities();

            _context.Orders.Update(Order);
            await _context.SaveChangesAsync();

            foreach (var (guid, quantity) in dishesInOrder)
            {
                var dish = _context.Dishes.Single(d => d.Id == guid);

                _context.DishInOrders.Add(new DishInOrder
                {
                    Dish = dish,
                    DishId = dish.Id,
                    Order = Order,
                    OrderId = Order.Id,
                    Quantity = quantity,
                });
            }

            await _context.SaveChangesAsync();
            Thread.Sleep(10000);
        }

        public Dictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(Order.FlatNumber))
            {
                errors["FlatNumber"] = "Address can't be empty!";
            }

            if (string.IsNullOrWhiteSpace(Order.FirstName))
            {
                errors["FirstName"] = "Name can't be empty!";
            }

            if (string.IsNullOrWhiteSpace(Order.LastName))
            {
                errors["LastName"] = "Surname can't be empty!";
            }

            return errors;
        }

        private Dictionary<Guid, int> GetDishesAndQuantities()
        {
            var dishesInOrder = new Dictionary<Guid, int>();

            foreach (var id in Dishes)
            {
                if (dishesInOrder.TryGetValue(id, out var quantity))
                {
                    dishesInOrder[id] = quantity + 1;
                }
                else
                {
                    dishesInOrder[id] = 1;
                }
            }

            return dishesInOrder;
        }
    }
}