using Envialo.Application.Abstractions;
using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IShipmentRepository : IRepository<ShipmentRequest>
{
    Task<IReadOnlyList<ShipmentRequest>> GetPendingAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ShipmentRequest>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default);
}