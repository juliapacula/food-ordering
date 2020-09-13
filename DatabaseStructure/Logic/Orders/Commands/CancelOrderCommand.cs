using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseStructure.Logic.Orders.Models;
using DatabaseStructure.Logic.Shared;

namespace DatabaseStructure.Logic.Orders.Commands
{
    public class CancelOrderCommand : ICommand
    {
        public Guid OrderId { get; set; }
        private DatabaseContext _context;

        public CancelOrderCommand(DatabaseContext context)
        {
            _context = context;
        }

        public async Task HandleAsync()
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == OrderId);

            if (order == null)
            {
                return;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}