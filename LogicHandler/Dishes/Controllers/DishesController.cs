using System.Collections.Generic;
using System.Linq;
using DatabaseStructure;
using DatabaseStructure.Logic.Dishes.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogicHandler.Dishes.Controllers
{
    [ApiController]
    [Route("/dishes")]
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