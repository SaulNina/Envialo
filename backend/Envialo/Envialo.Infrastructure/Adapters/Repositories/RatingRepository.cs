using Envialo.Domain.Entities;
using Envialo.Domain.Ports.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Envialo.Infrastructure.Adapters.Repositories;

public sealed class RatingRepository : BaseRepository<Rating>, IRatingRepository
{
    public RatingRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Rating>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default)
        => await Set.Where(r => r.TripId == tripId).ToListAsync(ct);

    public async Task<IEnumerable<Rating>> GetByUserIdAsync(Guid ratedUserId, CancellationToken ct = default)
        => await Set.Where(r => r.RatedUserId == ratedUserId).ToListAsync(ct);
}