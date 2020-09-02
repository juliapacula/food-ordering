using System.Collections.Generic;
using System.Linq;
using DatabaseStructure;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/dishes")]
    public class DishesController
    {
        private DatabaseContext _context;

        public DishesController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Dish> GetDishes()
        {
            return _context.Dishes.ToList();
        }
    }
}