using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface ITripRepository
{
    Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Trip>> GetByDriverIdAsync(Guid driverId, CancellationToken ct = default);
    Task AddAsync(Trip trip, CancellationToken ct = default);
}