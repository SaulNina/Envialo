using Envialo.Domain.DTOs.Shipments;
using Envialo.Domain.Entities;
using Envialo.Domain.Constants;
using Envialo.Domain.Ports.IRepositories;

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
            SuggestedPrice     = dto.SuggestedFare,
            Currency           = Currencies.Pen,             
            Status             = ShipmentStatuses.Open,           
            CreatedAt          = DateTime.UtcNow,
            UpdatedAt          = DateTime.UtcNow,
            ExpiresAt          = DateTime.UtcNow.AddHours(2) 
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