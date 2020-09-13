using Microsoft.EntityFrameworkCore;
using DatabaseStructure.Logic.Dishes.Models;
using DatabaseStructure.Logic.Orders.Models;

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
                order.Property(o => o.FirstName);
                order.Property(o => o.LastName);
                order.Property(o => o.Email);
                order.Property(o => o.PhoneNumber);
                order.Property(o => o.Street);
                order.Property(o => o.StreetNumber);
                order.Property(o => o.FlatNumber);
                order.Property(o => o.PostalCode);
                order.Property(o => o.Country);
                order.Property(o => o.Comment);
            });
            
            modelBuilder.Entity<Dish>(dish =>
            {
                dish.HasKey(d => d.Id);
                dish.Property(d => d.Name);
                dish.Property(d => d.Description);
                dish.Property(d => d.Price);
            });
        }
    }
}