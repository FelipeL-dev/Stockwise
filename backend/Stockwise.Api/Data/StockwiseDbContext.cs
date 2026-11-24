using Microsoft.EntityFrameworkCore;
using Stockwise.Api.Entities;

namespace Stockwise.Api.Data;


public class StockwiseDbContext : DbContext
{
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Sku)
                .IsUnique();

            entity.Property(p => p.Price)
                .HasPrecision(12, 2);

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasOne(sm => sm.Product)
                .WithMany(p => p.StockMovements)
                .HasForeignKey(sm => sm.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    
    public DbSet<StockMovement> StockMovements { get; set; } = null!;
    public StockwiseDbContext(DbContextOptions<StockwiseDbContext> options)
        : base(options)
    {
    }
}