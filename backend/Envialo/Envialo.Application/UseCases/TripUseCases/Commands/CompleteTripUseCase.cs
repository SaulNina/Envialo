using Envialo.Application.Abstractions;
using Envialo.Application.Ports;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.TripUseCases.Commands;

public sealed class CompleteTripUseCase
{
    private readonly ITripRepository     _trips;
    private readonly IShipmentRepository _shipments;
    private readonly IUnitOfWork         _uow;

    public CompleteTripUseCase(
        ITripRepository     trips,
        IShipmentRepository shipments,
        IUnitOfWork         uow)
    {
        _trips     = trips;
        _shipments = shipments;
        _uow       = uow;
    }

    public async Task ExecuteAsync(Guid tripId, CancellationToken ct = default)
    {
        var trip = await _trips.GetByIdAsync(tripId, ct)
                   ?? throw new DomainException($"Viaje '{tripId}' no encontrado.");

        if (trip.Status != "in_progress")
            throw new DomainException("Solo se puede completar un viaje en progreso.");

        var shipment = await _shipments.GetByIdAsync(trip.ShipmentId, ct)
                       ?? throw new ShipmentNotFoundException(trip.ShipmentId);

        trip.Status      = "completed";
        trip.CompletedAt = DateTime.UtcNow;
        shipment.Status  = "completed";

        await _trips.UpdateAsync(trip, ct);
        await _shipments.UpdateAsync(shipment, ct);
        await _uow.SaveChangesAsync(ct);
    }
}