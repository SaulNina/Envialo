using Envialo.Application.Abstractions;
using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IRatingRepository : IRepository<Rating>
{
    Task<IReadOnlyList<Rating>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);
    Task<IEnumerable<Rating>> GetByUserIdAsync(Guid ratedUserId, CancellationToken ct = default);
}