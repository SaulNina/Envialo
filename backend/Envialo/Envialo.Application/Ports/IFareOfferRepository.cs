using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IFareOfferRepository
{
    Task<FareOffer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<FareOffer>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken ct = default);
    Task AddAsync(FareOffer offer, CancellationToken ct = default);
    Task RejectAllExceptAsync(Guid shipmentId, Guid acceptedOfferId, CancellationToken ct = default);
}