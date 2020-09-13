using System;
using System.Collections.Generic;
using DatabaseStructure;
using DatabaseStructure.Logic.Orders.Models;
using DatabaseStructure.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Server.Orders.Mappers;
using Server.Orders.Models;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/orders")]
    public class OrderController
    {
        private QueueClient _queueClient;
        private IHubContext<OrderFulfillmentHub> _orderFulfillmentHub;

        public OrderController(QueueClient queueClient, IHubContext<OrderFulfillmentHub> orderFulfillmentHub)
        {
            _queueClient = queueClient;
            _orderFulfillmentHub = orderFulfillmentHub;
        }

        [HttpPost]
        public void AddOrder([FromBody] OrderWebModel order)
        {
            _queueClient.Publish(new FinalizeOrder()
            {
                Order = order.ToModel(),
                Dishes = order.Dishes,
            });
        }
    }
}