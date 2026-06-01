using Envialo.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Persistence.Repositories;

// L: todo repositorio específico puede sustituir a IRepository<T>
// DRY: lógica CRUD en un solo lugar
public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Db;
    protected readonly DbSet<T>    Set;

    protected BaseRepository(AppDbContext db)
    {
        Db  = db;
        Set = db.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Set.FindAsync(new object[] { id }, ct).AsTask();

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        => await Set.AddAsync(entity, ct);

    public virtual Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Set.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        Set.Remove(entity);
        return Task.CompletedTask;
    }
}