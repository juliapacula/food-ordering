using System;
using System.Collections.Generic;
using System.Net.WebSockets;
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

        public async Task InitializeOrder()
        {
            var orderId = Guid.NewGuid();
            if (Context.Items["orderId"] != null)
            {
                Context.Items["orderId"] = orderId;
            }
            else
            {
                Context.Items.Add("orderId", orderId);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, orderId.ToString());
            await Clients.Group(orderId.ToString()).SendAsync(OrderFulfillmentMessages.Init, orderId);

            _queueClient.Publish(new InitOrder()
            {
                OrderId = orderId,
            });
        }

        public async Task CancelOrder()
        {
            var orderId = (Guid) Context.Items["orderId"];

            await Clients.Caller.SendAsync(OrderFulfillmentMessages.Cancelled, orderId);

            _queueClient.Publish(new CancelOrder()
            {
                OrderId = orderId,
            });
        }
    }

    public struct OrderFulfillmentMessages
    {
        public static readonly string Init = "Init";
        public static readonly string Registered = "Registered";
        public static readonly string Completed = "Completed";
        public static readonly string Failed = "Failed";
        public static readonly string Error = "Error";
        public static readonly string Cancelled = "Cancelled";
    }
}