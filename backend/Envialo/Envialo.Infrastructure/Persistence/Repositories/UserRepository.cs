using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Persistence.Repositories;

// Adapter del hexágono: implementa el puerto IUserRepository
public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => Set.AnyAsync(u => u.Email == email.ToLower(), ct);
}