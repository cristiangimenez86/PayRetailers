using Microsoft.EntityFrameworkCore;
using PayRetailers.Domain.Entities;

namespace PayRetailers.Infrastructure.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Account).IsRequired();
            entity.Property(d => d.Type).IsRequired();
            entity.Property(d => d.Status).IsRequired();
            entity.Property(d => d.LastChange).IsRequired();
        });
    }
}
