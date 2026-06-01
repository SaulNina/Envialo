using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Persistence.Repositories;

public sealed class ShipmentRepository : BaseRepository<ShipmentRequest>, IShipmentRepository
{
    public ShipmentRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<ShipmentRequest>> GetPendingAsync(CancellationToken ct = default)
        => await Set.Where(s => s.Status == "pending")
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ShipmentRequest>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default)
        => await Set.Where(s => s.ClientId == clientId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
}