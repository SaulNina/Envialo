using Envialo.Application.Abstractions;
using Envialo.Application.DTOs.Shipments;
using Envialo.Application.Ports;
using Envialo.Domain.Entities;

namespace Envialo.Application.UseCases.ShipmentUseCases.Commands;

public sealed class CreateShipmentUseCase
{
    private readonly IShipmentRepository _shipments;
    private readonly IUnitOfWork         _uow;

    public CreateShipmentUseCase(IShipmentRepository shipments, IUnitOfWork uow)
    {
        _shipments = shipments;
        _uow       = uow;
    }

    public async Task<ShipmentResponseDto> ExecuteAsync(
        CreateShipmentDto dto, Guid clientId, CancellationToken ct = default)
    {
        var shipment = new ShipmentRequest
        {
            Id                 = Guid.NewGuid(),
            ClientId           = clientId,
            OriginAddress      = dto.OriginAddress,
            DestinationAddress = dto.DestinationAddress,
            CargoDescription   = dto.CargoDescription,
            WeightKg           = dto.WeightKg,
            SuggestedFare      = dto.SuggestedFare,
            Status             = "pending",
            CreatedAt          = DateTime.UtcNow
        };

        await _shipments.AddAsync(shipment, ct);
        await _uow.SaveChangesAsync(ct);

        return ToDto(shipment);
    }

    private static ShipmentResponseDto ToDto(ShipmentRequest s) => new(
        s.Id, s.OriginAddress, s.DestinationAddress,
        s.CargoDescription, s.WeightKg, s.SuggestedFare,
        s.Status, s.CreatedAt);
}