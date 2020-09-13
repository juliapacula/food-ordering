using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Dishes.Mappers;
using DatabaseStructure.Logic.Dishes.Models;
using Microsoft.AspNetCore.Mvc;
using Server.Dishes.Models;
using Server.Services;

namespace Server.Dishes.Controllers
{
    [ApiController]
    [Route("/api/dishes")]
    public class DishesController
    {
        private LogicHandlerClient _client;

        public DishesController(LogicHandlerClient logicHandlerClient)
        {
            _client = logicHandlerClient;
        }

        [HttpGet]
        public async Task<IEnumerable<DishWebModel>> GetDishesAsync()
        {
            var result = await _client.GetDishesAsync();
            return result.Select(d => d.ToWebModel());
        }
    }
}