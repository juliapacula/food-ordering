using System;
using DatabaseStructure.Logic.Orders.Models;
using Server.Orders.Models;

namespace Server.Orders.Mappers
{
    public static class OrderMapper
    {
        public static Order ToModel(this OrderWebModel model)
        {
            return new Order
            {
                Id = new Guid(model.Id),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Comment = model.Comment,
                Street = model.Street,
                StreetNumber = model.StreetNumber,
                FlatNumber = model.FlatNumber,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                PostalCode = model.PostalCode,
                Country = model.Country,
            };
        }
    }
}