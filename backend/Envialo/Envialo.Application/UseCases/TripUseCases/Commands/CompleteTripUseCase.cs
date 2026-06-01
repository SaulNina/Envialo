using Envialo.Domain.Interfaces;

namespace Envialo.Application.UseCases.TripUseCases.Commands;

public record CompleteTripCommand(Guid TripId, Guid DriverId);

public class CompleteTripHandler(ITripRepository tripRepo, IShipmentRepository shipmentRepo)
{
    public async Task HandleAsync(CompleteTripCommand cmd, CancellationToken ct = default)
    {
        var trip = await tripRepo.GetByIdAsync(cmd.TripId, ct)
                   ?? throw new KeyNotFoundException("Viaje no encontrado.");

        if (trip.DriverId != cmd.DriverId)
            throw new UnauthorizedAccessException("Solo el conductor puede completar el viaje.");

        var shipment = await shipmentRepo.GetByIdAsync(trip.ShipmentId, ct);

        trip.Status      = "completed";
        trip.CompletedAt = DateTime.UtcNow;

        if (shipment is not null) shipment.Status = "completed";

        await tripRepo.SaveChangesAsync(ct);
    }
}