using DatabaseStructure.Logic.Orders.Models;
using DatabaseStructure.Messages;

namespace DatabaseStructure
{
    public static class Tools
    {
        public static string Validate(this Order order, FinalizeOrder finalizeOrder)
        {
            var errors = string.Empty;

            // if (finalizeOrder.DishesAndQuantity == null || finalizeOrder.DishesAndQuantity.Count == 0)
            //     errors += "Order is empty!";

            if (string.IsNullOrWhiteSpace(order.Street))
                errors += "Address can't be empty!";

            if (string.IsNullOrWhiteSpace(order.FirstName))
                errors += "Name can't be empty!";

            if (string.IsNullOrWhiteSpace(order.LastName))
                errors += "Surname can't be empty!";


            return errors;
        }
    }
}
