using Envialo.Domain.Entities;
using Envialo.Domain.Ports.IServices;

namespace Envialo.Domain.Ports.IRepositories;

public interface IRatingRepository : IRepository<Rating>
{
    Task<IReadOnlyList<Rating>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);
    Task<IEnumerable<Rating>> GetByUserIdAsync(Guid ratedUserId, CancellationToken ct = default);
}