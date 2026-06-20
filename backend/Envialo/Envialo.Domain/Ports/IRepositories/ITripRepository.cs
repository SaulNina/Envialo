using Envialo.Domain.Entities;
using Envialo.Domain.Ports.IServices;

namespace Envialo.Domain.Ports.IRepositories;

public interface ITripRepository : IRepository<Trip>
{
    Task<Trip?> GetByShipmentIdAsync(Guid shipmentId, CancellationToken ct = default);
}