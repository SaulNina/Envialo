using Envialo.Application.Abstractions;
using Envialo.Application.Ports;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.FareOfferUseCases.Commands;

public sealed class AcceptFareOfferUseCase
{
    private readonly IFareOfferRepository _offers;
    private readonly IShipmentRepository  _shipments;
    private readonly IUnitOfWork          _uow;

    public AcceptFareOfferUseCase(
        IFareOfferRepository offers,
        IShipmentRepository  shipments,
        IUnitOfWork          uow)
    {
        _offers    = offers;
        _shipments = shipments;
        _uow       = uow;
    }

    public async Task ExecuteAsync(Guid offerId, Guid clientId, CancellationToken ct = default)
    {
        var offer = await _offers.GetByIdAsync(offerId, ct)
                    ?? throw new DomainException($"Oferta '{offerId}' no encontrada.");

        var shipment = await _shipments.GetByIdAsync(offer.ShipmentId, ct)
                       ?? throw new ShipmentNotFoundException(offer.ShipmentId);

        if (shipment.ClientId != clientId)
            throw new UnauthorizedDomainException("No puedes aceptar una oferta de otro cliente.");

        offer.Status    = "ACCEPTED";
        shipment.Status = "ACCEPTED";

        await _offers.UpdateAsync(offer, ct);
        await _shipments.UpdateAsync(shipment, ct);
        await _uow.SaveChangesAsync(ct);
    }
}