using Microsoft.EntityFrameworkCore;

namespace OrderService.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    DbSet<T> Set<T>() where T : class;

    Task AddRangeAsync<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : class;
    
    Task UpdateRangeAsync<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : class;
}