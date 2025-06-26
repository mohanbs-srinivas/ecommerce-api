using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ecommerce_api.Models;

namespace ecommerce_api.Data
{
    public class EcommerceContext : IdentityDbContext<ApplicationUser>
    {
        public EcommerceContext(DbContextOptions<EcommerceContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure inheritance for ApplicationUser
            builder.Entity<Member>().HasBaseType<ApplicationUser>();
            builder.Entity<Staff>().HasBaseType<ApplicationUser>();
        }
    }
}