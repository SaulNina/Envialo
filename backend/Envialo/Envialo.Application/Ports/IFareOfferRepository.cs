using Envialo.Application.Abstractions;
using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IFareOfferRepository : IRepository<FareOffer>
{
    Task<IReadOnlyList<FareOffer>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken ct = default);
}