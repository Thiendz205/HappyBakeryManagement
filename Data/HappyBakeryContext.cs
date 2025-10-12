using HappyBakeryManagement.Models;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Data
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
    }
}
