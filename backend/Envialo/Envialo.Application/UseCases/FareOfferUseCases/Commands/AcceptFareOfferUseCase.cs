using Envialo.Application.Abstractions;

namespace Envialo.Application.UseCases.FareOfferUseCases.Commands;

public record AcceptFareOfferInput(Guid OfferId, Guid ClientId);

public class AcceptFareOfferUseCase(IUnitOfWork uow)
{
    public async Task ExecuteAsync(AcceptFareOfferInput input, CancellationToken ct = default)
    {
        var offer = await uow.FareOffers.GetByIdAsync(input.OfferId, ct)
                    ?? throw new ShipmentNotFoundException(input.OfferId);

        var shipment = await uow.Shipments.GetByIdAsync(offer.ShipmentId, ct)
                       ?? throw new ShipmentNotFoundException(offer.ShipmentId);

        if (shipment.ClientId != input.ClientId)
            throw new UnauthorizedDomainException("Solo el cliente dueño puede aceptar ofertas.");

        // Todo en una sola transacción
        await uow.FareOffers.RejectAllExceptAsync(shipment.Id, offer.Id, ct);
        offer.Status    = "accepted";
        shipment.Status = "accepted";

        await uow.CommitAsync(ct);  // ← atomicidad garantizada
    }
}