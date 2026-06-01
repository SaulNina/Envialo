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
            SuggestedPrice     = dto.SuggestedFare, // ¡Corregido! Mapea a SuggestedPrice
            Currency           = "PEN",             // ¡Obligatorio por tu DB!
            Status             = "OPEN",            // ¡Corregido! Debe coincidir con el CHECK constraint
            CreatedAt          = DateTime.UtcNow,
            UpdatedAt          = DateTime.UtcNow,
            ExpiresAt          = DateTime.UtcNow.AddHours(2) // La BD exige expires_at
        };

        await _shipments.AddAsync(shipment, ct);
        await _uow.SaveChangesAsync(ct);

        return ToDto(shipment);
    }

    private static ShipmentResponseDto ToDto(ShipmentRequest s) => new(
        s.Id, s.OriginAddress, s.DestinationAddress,
        s.CargoDescription, s.WeightKg, s.SuggestedPrice ?? 0m, // Corregido aquí también
        s.Status, s.CreatedAt);
}