using Envialo.Domain.Constants;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.TripUseCases.Commands;

public sealed class CancelTripCommand
{
    private readonly ITripRepository     _trips;
    private readonly IShipmentRepository _shipments;
    private readonly IUnitOfWork         _uow;

    public CancelTripCommand(ITripRepository trips, IShipmentRepository shipments, IUnitOfWork uow)
    {
        _trips     = trips;
        _shipments = shipments;
        _uow       = uow;
    }

    public async Task ExecuteAsync(Guid tripId, Guid userId, string reason, CancellationToken ct = default)
    {
        var trip = await _trips.GetByIdAsync(tripId, ct)
                   ?? throw new DomainException("Viaje no encontrado.");

        // Permitir cancelar tanto al cliente dueño como al conductor asignado
        var shipment = await _shipments.GetByIdAsync(trip.ShipmentId, ct)
                       ?? throw new DomainException("Flete asociado no encontrado.");

        if (trip.DriverId != userId && shipment.ClientId != userId)
            throw new UnauthorizedDomainException("No estás vinculado a este viaje.");

        if (trip.Status == TripStatuses.Completed || trip.Status == TripStatuses.Cancelled)
            throw new DomainException($"No se puede cancelar un viaje en estado {trip.Status}.");

        // Al cancelar el viaje, liberamos el flete original devolviéndolo a "OPEN"
        // para que otros conductores puedan ofertar, o lo cancelamos según tu regla.
        // Aquí lo dejamos en CANCELLED también para cerrar el flujo completo.
        trip.Status = TripStatuses.Cancelled;
        trip.CancelReason = reason;
        
        shipment.Status = ShipmentStatuses.Cancelled;
        shipment.CancelReason = $"Viaje cancelado: {reason}";

        await _trips.UpdateAsync(trip, ct);
        await _shipments.UpdateAsync(shipment, ct);
        await _uow.SaveChangesAsync(ct);
    }
}