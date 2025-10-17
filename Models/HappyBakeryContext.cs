using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Models
{
    public class HappyBakeryContext : DbContext
    {
        public HappyBakeryContext()
        {

        }

        public HappyBakeryContext(DbContextOptions<HappyBakeryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Evaluate> Evaluates { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }

    }
}
