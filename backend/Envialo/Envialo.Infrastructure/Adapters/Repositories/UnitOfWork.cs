using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Infrastructure.Adapters.Repositories;

// OCP: no necesita modificarse si se agregan nuevos repositorios
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}