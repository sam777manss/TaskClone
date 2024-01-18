using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoesApi.DbContextFile.DBFiles;
using ShoesApi.Models;

namespace ShoesApi.DbContextFile
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public virtual DbSet<AddProductTable> AddProductTable { get; set; }
        public virtual DbSet<ProductImageTable> ProductImageTable { get; set; }
        public virtual DbSet<UserCart> UserCart { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AddProductTable>()
                .HasMany(e => e.ProductImageTable) // many images
                .WithOne(z => z.AddProductTables)  // one product 
                .OnDelete(DeleteBehavior.Cascade); // when product deleted corresponding primary key as foregin key holder data will be deleted

            builder.Entity<AddProductTable>()
                .HasKey(d => d.ProductId); // AddProductTable key

            builder.Entity<ProductImageTable>()
                .HasOne(e => e.AddProductTables) // one product 
                .WithMany(e => e.ProductImageTable) // many images
                .HasForeignKey(e => e.ProductId).IsRequired(); // ProductImageTable has foreign key as ProductId referencing from AddProductTable

            builder.Entity<ProductImageTable>()
                .HasKey(d => d.ProductImgId); // ProductImageTable key

            builder.Entity<UserCart>()
                .HasOne(e => e.addProductTables)
                .WithMany(z => z.UserCart)
                .HasForeignKey(e => e.ProductId);

            base.OnModelCreating(builder);
        }

    }
}
