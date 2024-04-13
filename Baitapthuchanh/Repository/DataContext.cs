using Baitapthuchanh.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Baitapthuchanh.Repository
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
 
        }
        public DbSet<Category> Categories { get; set;}
        public DbSet<Product> Products { get; set;}
        public DbSet<ProductImage> ProductImages { get; set;}
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //	base.OnModelCreating(modelBuilder);
        //      }

    }
}
