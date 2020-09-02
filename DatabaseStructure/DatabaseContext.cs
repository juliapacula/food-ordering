using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DatabaseStructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }

        public DbSet<DishInOrder> DishesInOrders { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
    }
}