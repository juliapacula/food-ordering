using Microsoft.EntityFrameworkCore;
using DatabaseStructure.EntitySets;

namespace DatabaseStructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishInOrder> DishInOrders { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DishInOrder>(dishInOrder =>
            {
                dishInOrder.HasKey(d => new {d.DishId, d.OrderId});

                dishInOrder.HasOne(d => d.Order)
                    .WithMany()
                    .HasForeignKey(d => d.OrderId)
                    .IsRequired();

                dishInOrder.HasOne(d => d.Dish)
                    .WithMany()
                    .HasForeignKey(d => d.DishId)
                    .IsRequired();
            });
            
            modelBuilder.Entity<Order>(order =>
            {
                order.HasKey(o => o.Id);
            });
            
            modelBuilder.Entity<Dish>(dish =>
            {
                dish.HasKey(d => d.Id);
            });
        }
    }
}