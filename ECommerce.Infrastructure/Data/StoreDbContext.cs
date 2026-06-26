using ECommerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Data
{
    public class StoreDbContext:DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options):base(options)
        {
            
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductsBrand> ProductBrands { get; set; }
        public DbSet<ProductsType> ProductTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);
        }
    }
}
