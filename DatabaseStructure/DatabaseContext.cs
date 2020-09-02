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
}