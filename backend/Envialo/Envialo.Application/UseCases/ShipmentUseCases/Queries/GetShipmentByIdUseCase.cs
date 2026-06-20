using Envialo.Domain.DTOs.Shipments;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.ShipmentUseCases.Queries;

public sealed class GetShipmentByIdUseCase
{
    private readonly IShipmentRepository _shipments;

    public GetShipmentByIdUseCase(IShipmentRepository shipments) => _shipments = shipments;

    public async Task<ShipmentResponseDto> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _shipments.GetByIdAsync(id, ct)
                ?? throw new ShipmentNotFoundException(id);

        return new ShipmentResponseDto(
            s.Id, s.OriginAddress, s.DestinationAddress,
            s.CargoDescription, s.WeightKg, s.SuggestedPrice ?? 0m,
            s.Status, s.CreatedAt);
    }
}