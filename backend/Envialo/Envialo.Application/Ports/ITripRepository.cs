using Envialo.Application.Abstractions;
using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface ITripRepository : IRepository<Trip>
{
    Task<Trip?> GetByShipmentIdAsync(Guid shipmentId, CancellationToken ct = default);
}