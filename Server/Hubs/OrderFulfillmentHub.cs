using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseStructure.Messages;
using Microsoft.AspNetCore.SignalR;
using Server.Services;

namespace Server.Hubs
{
    public class OrderFulfillmentHub : Hub
    {
        private QueueClient _queueClient;

        public OrderFulfillmentHub(QueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public override async Task OnConnectedAsync()
        {
            var orderId = Guid.NewGuid();
            Context.Items.Add("orderId", orderId);
            await Clients.Caller.SendAsync(OrderFulfillmentMessages.Init, orderId);

            _queueClient.Publish(new InitOrder()
            {
                OrderId = orderId,
            });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var orderId = (Guid)Context.Items["orderId"];

            await Clients.Caller.SendAsync(OrderFulfillmentMessages.Cancelled, orderId);

            _queueClient.Publish(new CancelOrder()
            {
                OrderId = orderId,
            });

            await base.OnDisconnectedAsync(exception);
        }
    }

    public struct OrderFulfillmentMessages
    {
        public static readonly string Init = "Init";
        public static readonly string Registered = "Registered";
        public static readonly string Processed = "Processed";
        public static readonly string Completed = "Completed";
        public static readonly string Failed = "Failed";
        public static readonly string Cancelled = "Cancelled";
    }
}