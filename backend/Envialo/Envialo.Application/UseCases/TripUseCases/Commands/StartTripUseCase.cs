using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Envialo.Domain.Interfaces;

namespace Envialo.Application.UseCases.TripUseCases.Commands;

public record StartTripCommand(Guid ShipmentId, Guid DriverId);

public class StartTripHandler(ITripRepository tripRepo, IShipmentRepository shipmentRepo)
{
    public async Task<Trip> HandleAsync(StartTripCommand cmd, CancellationToken ct = default)
    {
        var shipment = await shipmentRepo.GetByIdAsync(cmd.ShipmentId, ct)
                       ?? throw new KeyNotFoundException("Solicitud no encontrada.");

        if (shipment.Status != "accepted")
            throw new InvalidOperationException("La solicitud debe estar aceptada para iniciar el viaje.");

        var trip = new Trip
        {
            Id         = Guid.NewGuid(),
            ShipmentId = cmd.ShipmentId,
            DriverId   = cmd.DriverId,
            Status     = "in_progress",
            StartedAt  = DateTime.UtcNow
        };

        shipment.Status = "in_progress";

        await tripRepo.AddAsync(trip, ct);
        await tripRepo.SaveChangesAsync(ct);
        return trip;
    }
}