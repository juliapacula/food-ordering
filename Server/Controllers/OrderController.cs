using System;
using DatabaseStructure;
using Microsoft.AspNetCore.Mvc;
using Server.WebModels;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/orders")]
    public class OrderController
    {
        private DatabaseContext _context;

        public OrderController(DatabaseContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public void AddOrder(OrderWebModel order)
        {
            Console.WriteLine(order.Comment);
        }
    }
}