using Envialo.Domain.Ports.IRepositories;
using Envialo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Adapters.Repositories;

public sealed class TripRepository : BaseRepository<Trip>, ITripRepository
{
    public TripRepository(AppDbContext db) : base(db) { }

    public Task<Trip?> GetByShipmentIdAsync(Guid shipmentId, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(t => t.ShipmentId == shipmentId, ct);
    
    public async Task<IReadOnlyList<Trip>> GetByDriverIdAsync(Guid driverId, CancellationToken ct = default)
    {
        return await Set
            .Include(t => t.Shipment) // Traemos los datos de la carga asociada
            .Where(t => t.DriverId == driverId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }
}