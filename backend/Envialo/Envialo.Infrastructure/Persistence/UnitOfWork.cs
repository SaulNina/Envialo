using Envialo.Application.Abstractions;

namespace Envialo.Infrastructure.Persistence;

// OCP: no necesita modificarse si se agregan nuevos repositorios
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}