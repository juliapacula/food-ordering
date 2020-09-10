using System;
using System.Collections.Generic;
using DatabaseStructure;
using DatabaseStructure.EntitySets;
using DatabaseStructure.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Server.Services;
using Server.WebModels;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/orders")]
    public class OrderController
    {
        private DatabaseContext _context;
        private QueueClient _queueClient;
        private IHubContext<OrderFulfillmentHub> _orderFulfillmentHub;

        public OrderController(
            QueueClient queueClient,
            DatabaseContext context,
            IHubContext<OrderFulfillmentHub> orderFulfillmentHub
        )
        {
            _queueClient = queueClient;
            _context = context;
            _orderFulfillmentHub = orderFulfillmentHub;
        }

        [HttpPost]
        public void AddOrder(OrderWebModel order)
        {
            var dishesInOrder = new Dictionary<Guid, int>();

            foreach (var dish in order.Dishes)
            {
                if (dishesInOrder.TryGetValue(dish.Id, out var quantity))
                {
                    dishesInOrder[dish.Id] = quantity + 1;
                }
                else
                {
                    dishesInOrder[dish.Id] = 1;
                }
            }

            _queueClient.Publish(new FinalizeOrder()
            {
                Order = new Order()
                {
                    Id = new Guid(order.Id),
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    Comment = order.Comment,
                    Street = order.Street,
                    StreetNumber = order.StreetNumber,
                    FlatNumber = order.FlatNumber,
                    PhoneNumber = order.PhoneNumber,
                    Email = order.Email,
                    PostalCode = order.PostalCode,
                    Country = order.Country,
                },
                DishesAndQuantity = dishesInOrder,
            });
        }
    }
}