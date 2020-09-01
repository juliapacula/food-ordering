using System;
using Microsoft.EntityFrameworkCore;

namespace DatabaseStructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
    }

    public class Dish
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}