using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Persistence.Repositories;

public sealed class RatingRepository : BaseRepository<Rating>, IRatingRepository
{
    public RatingRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Rating>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default)
        => await Set.Where(r => r.TripId == tripId).ToListAsync(ct);
}