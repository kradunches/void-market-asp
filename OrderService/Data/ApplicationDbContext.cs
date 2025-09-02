using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<Item> Items { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public async Task AddRangeAsync<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : class
    {
        await Set<TEntity>().AddRangeAsync(entities);
    }

    public async Task UpdateRangeAsync<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : class
    {
        await Task.Run(() =>
        {
            Set<TEntity>().UpdateRange(entities);
            return Task.CompletedTask;
        });
    }
}