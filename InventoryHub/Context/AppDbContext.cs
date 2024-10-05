using InventoryHub.Models;
using InventoryHub.ServicesPatterns.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Context
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.HasDefaultSchema("");

            builder.Entity<ApplicationUser>().ToTable("Users", "Security");
            builder.Entity<IdentityRole>().ToTable("Roles", "Security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Security");
            // .Ignore(idx=>idx.PhoneNumberConfirmed);


            // Primary Key 
            //builder.Entity<Category>()
            //    .HasKey(idx=>idx.CategoryId);
            //builder.Entity<Product>()
            //    .HasKey(idx=>idx.ProductId);
            //builder.Entity<Store>()
            //    .HasKey(idx=>idx.Id);
            //builder.Entity<AuditableEntity>()
            //    .HasKey(e => e.Id);

            //builder.Entity<Category>()
            //    .ToTable("Categories");

            //builder.Entity<Store>()
            //    .ToTable("Stores");


            // Global Query
            builder.Entity<Product>()
                .HasQueryFilter(idx => idx.IsDeleted != true);
            builder.Entity<Category>()
               .HasQueryFilter(idx => idx.IsDeleted != true);
            builder.Entity<Store>()
               .HasQueryFilter(idx => idx.IsDeleted != true);
            builder.Entity<Customer>()
              .HasQueryFilter(idx => idx.IsDeleted != true);
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
