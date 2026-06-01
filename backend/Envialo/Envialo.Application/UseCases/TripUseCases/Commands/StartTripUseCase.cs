using Envialo.Application.Abstractions;
using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.TripUseCases.Commands;

public sealed class StartTripUseCase
{
    private readonly ITripRepository     _trips;
    private readonly IShipmentRepository _shipments;
    private readonly IUnitOfWork         _uow;

    public StartTripUseCase(
        ITripRepository     trips,
        IShipmentRepository shipments,
        IUnitOfWork         uow)
    {
        _trips     = trips;
        _shipments = shipments;
        _uow       = uow;
    }

    public async Task<Trip> ExecuteAsync(Guid shipmentId, Guid driverId, CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(shipmentId, ct)
                       ?? throw new ShipmentNotFoundException(shipmentId);

        if (shipment.Status != "assigned")
            throw new DomainException("El envío debe estar asignado para iniciar el viaje.");

        var trip = new Trip
        {
            Id         = Guid.NewGuid(),
            ShipmentId = shipmentId,
            DriverId   = driverId,
            Status     = "in_progress",
            StartedAt  = DateTime.UtcNow
        };

        shipment.Status = "in_progress";
        await _trips.AddAsync(trip, ct);
        await _shipments.UpdateAsync(shipment, ct);
        await _uow.SaveChangesAsync(ct);
        return trip;
    }
}