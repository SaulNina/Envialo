using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IRatingRepository
{
    Task<IEnumerable<Rating>> GetByRatedUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Rating rating, CancellationToken ct = default);
}