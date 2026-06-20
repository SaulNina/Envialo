using Envialo.Domain.Entities;
using Envialo.Domain.Ports.IServices;

namespace Envialo.Domain.Ports.IRepositories;

public interface IFareOfferRepository : IRepository<FareOffer>
{
    Task<IReadOnlyList<FareOffer>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken ct = default);
}