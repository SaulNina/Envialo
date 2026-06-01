using Envialo.Application.Abstractions;
using Envialo.Domain.Entities;

namespace Envialo.Application.UseCases.ShipmentUseCases.Commands;

public record CreateShipmentInput(
    Guid    ClientId,
    string  OriginAddress,
    string  DestinationAddress,
    double  OriginLat,    double OriginLng,
    double  DestinationLat, double DestinationLng,
    string  CargoDescription,
    decimal ClientBudget
);

public class CreateShipmentUseCase(IUnitOfWork uow)
{
    public async Task<ShipmentRequest> ExecuteAsync(CreateShipmentInput input, CancellationToken ct = default)
    {
        var shipment = new ShipmentRequest
        {
            Id                 = Guid.NewGuid(),
            ClientId           = input.ClientId,
            OriginAddress      = input.OriginAddress,
            DestinationAddress = input.DestinationAddress,
            OriginLat          = input.OriginLat,
            OriginLng          = input.OriginLng,
            DestinationLat     = input.DestinationLat,
            DestinationLng     = input.DestinationLng,
            CargoDescription   = input.CargoDescription,
            ClientBudget       = input.ClientBudget,
            Status             = "pending",
            CreatedAt          = DateTime.UtcNow
        };

        await uow.Shipments.AddAsync(shipment, ct);
        await uow.CommitAsync(ct);              // ← un solo commit
        return shipment;
    }
}