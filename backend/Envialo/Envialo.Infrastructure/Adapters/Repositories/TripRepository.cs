using Envialo.Domain.Ports.IRepositories;
using Envialo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Adapters.Repositories;

public sealed class TripRepository : BaseRepository<Trip>, ITripRepository
{
    public TripRepository(AppDbContext db) : base(db) { }

    public Task<Trip?> GetByShipmentIdAsync(Guid shipmentId, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(t => t.ShipmentId == shipmentId, ct);
}