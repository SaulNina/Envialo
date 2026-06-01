using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IShipmentRepository
{
    Task<ShipmentRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ShipmentRequest>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default);
    Task<IEnumerable<ShipmentRequest>> GetPendingNearbyAsync(double lat, double lng, double radiusKm, CancellationToken ct = default);
    Task AddAsync(ShipmentRequest shipment, CancellationToken ct = default);
}