using Envialo.Domain.Entities;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.FareOfferUseCases.Queries;

public sealed class GetOffersByShipmentUseCase
{
    private readonly IFareOfferRepository _offers;

    public GetOffersByShipmentUseCase(IFareOfferRepository offers) => _offers = offers;

    public Task<IReadOnlyList<FareOffer>> ExecuteAsync(Guid shipmentId, CancellationToken ct = default)
        => _offers.GetByShipmentIdAsync(shipmentId, ct);
}