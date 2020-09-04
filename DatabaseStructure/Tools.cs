using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using DatabaseStructure.EntitySets;
using DatabaseStructure.Messages;

namespace DatabaseStructure
{
    public static class Tools
    {
        public static string Validate(this Order order, FinalizeOrder finalizeOrder)
        {
            var errors = string.Empty;

            if (finalizeOrder.dishesAndQuantity == null || finalizeOrder.dishesAndQuantity.Count == 0)
                errors += "Order is empty!";

            if (string.IsNullOrWhiteSpace(order.Address))
                errors += "Address can't be empty!";

            if (string.IsNullOrWhiteSpace(order.Name))
                errors += "Name can't be empty!";

            if (string.IsNullOrWhiteSpace(order.Surname))
                errors += "Surname can't be empty!";

            try
            {
                _ = new MailAddress(order.Email).Address;
            }
            catch (FormatException)
            {
                errors += "Invalid email!";
            }

            return errors;
        }
    }
}
