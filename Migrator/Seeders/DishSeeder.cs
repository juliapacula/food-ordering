using System;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure;
using Microsoft.EntityFrameworkCore;

namespace Migrator.Seeders
{
    internal static class DishSeeder
    {
        private static readonly string[] DishIds =
        {
            "3488cf09-c231-45c1-8caa-ea709dbc7a2d",
            "d2ab88e1-f311-436e-bca8-a622b4d5186f",
            "65fe4163-3608-4a40-849f-c187bb45d0f9",
            "7aaa4905-d7dc-4e65-9210-4d3960c1bd11",
            "fb5977df-9728-4929-9145-37ceaeca742c",
            "b473c4c8-5fe2-4d97-a352-9e4a706777a2",
            "9d38ab51-ae0a-4084-a88b-77551b03c95b",
            "3d4e3aec-88c6-4e9d-9f00-9d1ec84eb9fe",
            "a226e19d-9030-4a41-8a6b-d692108892c5",
            "3cb2915a-3405-4d67-9902-49dd9828341d",
        };

        public static async Task SeedDishesAsync(this DatabaseContext context, CancellationToken cancellationToken)
        {
            Dish[] dishes =
            {
                new Dish
                {
                    Description = "Z mięsem Pastor, boczkiem, papryką zieloną, papryką czerwoną, cebulą i serem Cheddar.",
                    Name = "Alambre Pastor",
                    Price = 28.00
                },
                new Dish
                {
                    Description = "Smażona tortilla kukurydziana z kurczakiem duszonym, serem, śmietaną, sałatą, sosem zielonym i sosem czerwonym.",
                    Name = "Enchiladas",
                    Price = 25.00
                },
                new Dish
                {
                    Description = "Ze smażoną tortillą, sosem serowym Cheddar, zielonym chilli i śmietaną.",
                    Name = "Nachos",
                    Price = 20.00
                },
                new Dish
                {
                    Description = "Smażona tortilla kukurydziana z kurczakiem duszonym, sosem czerwonym, serem, śmietaną, cebulą i kolendrą.",
                    Name = "Chilaquiles Czerwone",
                    Price = 28.00
                },
                new Dish
                {
                    Description = "Smażona tortilla kukurydziana z kurczakiem duszonym, sosem zielonym, serem, śmietaną, cebulą i kolendrą.",
                    Name = "Chilaquiles Zielone",
                    Price = 28.00
                },
                new Dish
                {
                    Description = "Ze smażoną tortillą, sosem serowym Cheddar, zielonym chilli, śmietaną, Guacamole i mięsem Pastor.",
                    Name = "Nachos Pastor",
                    Price = 25.00
                },
                new Dish
                {
                    Description = "Ze smażoną tortillą, sosem serowym Cheddar, zielonym chilli i kwaśną śmietaną.",
                    Name = "Nachos z Chilli",
                    Price = 25.00
                },
                new Dish
                {
                    Description = "Smażona tortilla kukurydziana z kurczakiem duszonym, sałatą, śmietaną, serem, cebulą, kolendrą i sosem do wyboru.",
                    Name = "Złote Tacos",
                    Price = 25.00
                },
                new Dish
                {
                    Description = "Z mięsem z kurczaka, boczkiem, papryką zieloną, papryką czerwoną, cebulą i serem Cheddar.",
                    Name = "Alambre z Kebabem",
                    Price = 23.00
                },
                new Dish
                {
                    Description = "Z wołowiną, boczkiem, papryką zieloną, papryką czerwoną, cebulą i serem Cheddar.",
                    Name = "Alambre z Wołowiną",
                    Price = 28.00
                },
            };

            var index = 0;
            foreach (var dish in dishes)
            {
                dish.Id = new Guid(DishIds[index++]);

                var dishExists = await context.Dishes.AnyAsync(d => d.Id == dish.Id, cancellationToken);

                if (!dishExists)
                {
                    await context.Dishes.AddAsync(dish, cancellationToken);
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}