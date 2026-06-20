using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Constants;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.FareOfferUseCases.Commands;

public sealed class CreateFareOfferCommand
{
    private readonly IFareOfferRepository  _offers;
    private readonly IShipmentRepository   _shipments;
    private readonly IUnitOfWork           _uow;

    public CreateFareOfferCommand(
        IFareOfferRepository offers,
        IShipmentRepository  shipments,
        IUnitOfWork          uow)
    {
        _offers    = offers;
        _shipments = shipments;
        _uow       = uow;
    }

    public async Task<FareOffer> ExecuteAsync(
        Guid shipmentId, Guid driverId, decimal amount, CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(shipmentId, ct)
                       ?? throw new ShipmentNotFoundException(shipmentId);

        if (shipment.Status != ShipmentStatuses.Open)
            throw new DomainException("Solo se pueden hacer ofertas en envíos abiertos.");

        var offer = new FareOffer
        {
            Id           = Guid.NewGuid(),
            ShipmentId   = shipmentId,
            DriverId     = driverId,
            OfferedPrice = amount,   
            Currency     = Currencies.Pen,      
            Status       = OfferStatuses.Pending,   
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };

        await _offers.AddAsync(offer, ct);
        await _uow.SaveChangesAsync(ct);
        return offer;
    }
}