using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Persistence.Repositories;

public sealed class FareOfferRepository : BaseRepository<FareOffer>, IFareOfferRepository
{
    public FareOfferRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<FareOffer>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken ct = default)
        => await Set.Where(o => o.ShipmentId == shipmentId)
            .OrderBy(o => o.Amount)
            .ToListAsync(ct);
}