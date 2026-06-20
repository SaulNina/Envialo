using Envialo.Domain.Entities;
using Envialo.Domain.Ports.IServices;

namespace Envialo.Domain.Ports.IRepositories;

public interface IShipmentRepository : IRepository<ShipmentRequest>
{
    Task<IReadOnlyList<ShipmentRequest>> GetPendingAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ShipmentRequest>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default);
}