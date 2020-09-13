using DatabaseStructure.Logic.Dishes.Models;
using Server.Dishes.Models;

namespace Server.Dishes.Mappers
{
    public static class DishMapper
    {
        public static DishWebModel ToWebModel(this Dish model)
        {
            return new DishWebModel()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
            };
        }
    }
}