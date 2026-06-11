using Envialo.Application.Abstractions;
using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Constants;

namespace Envialo.Application.UseCases.FareOfferUseCases.Commands;

public sealed class AcceptFareOfferUseCase
{
    private readonly IFareOfferRepository _offers;
    private readonly IShipmentRepository  _shipments;
    private readonly ITripRepository _trips;
    private readonly IUnitOfWork          _uow;

    public AcceptFareOfferUseCase(
        IFareOfferRepository offers,
        IShipmentRepository  shipments,
        ITripRepository  trips,
        IUnitOfWork          uow)
    {
        _offers    = offers;
        _shipments = shipments;
        _trips     = trips;
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

        offer.Status    = OfferStatuses.Accepted;
        shipment.Status = ShipmentStatuses.Accepted;

        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipment.Id,
            DriverId = offer.DriverId,
            AcceptedOfferId = offer.Id,
            FinalPrice = offer.OfferedPrice,
            Status = TripStatuses.Confirmed
        };

        await _offers.UpdateAsync(offer, ct);
        await _shipments.UpdateAsync(shipment, ct);
        await _trips.AddAsync(trip, ct);
        
        await _uow.SaveChangesAsync(ct);
    }
}