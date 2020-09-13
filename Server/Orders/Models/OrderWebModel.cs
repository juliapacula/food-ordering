using System;
using System.Collections.Generic;

namespace Server.Orders.Models
{
    public class OrderWebModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string FlatNumber { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Comment { get; set; }
        public IEnumerable<Guid> Dishes { get; set; }
    }
}