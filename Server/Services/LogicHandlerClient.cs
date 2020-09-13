using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DatabaseStructure.Logic.Dishes.Models;
using Newtonsoft.Json;

namespace Server.Services
{
    public class LogicHandlerClient
    {
        private HttpClient _client;

        public LogicHandlerClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Dish>> GetDishesAsync()
        {
            var result = await _client.GetStringAsync("dishes");

            return JsonConvert.DeserializeObject<IEnumerable<Dish>>(result);
        }
    }
}