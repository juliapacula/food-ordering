using System;
using System.Threading.Tasks;
using DatabaseStructure.Logic.Orders.Models;
using DatabaseStructure.Logic.Shared;

namespace DatabaseStructure.Logic.Orders.Commands
{
    public class InitOrderCommand: ICommand
    {
        public Guid OrderId { get; set; }
        private DatabaseContext _context;

        public InitOrderCommand(DatabaseContext context)
        {
            _context = context;
        }

        public async Task HandleAsync()
        {
            _context.Orders.Add(new Order
            {
                Id = OrderId
            });
            await _context.SaveChangesAsync();
        }
    }
}