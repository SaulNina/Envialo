using Envialo.Application.DTOs.Shipments;
using Envialo.Application.Ports;

namespace Envialo.Application.UseCases.ShipmentUseCases.Queries;

public sealed class GetPendingShipmentsUseCase
{
    private readonly IShipmentRepository _shipments;

    public GetPendingShipmentsUseCase(IShipmentRepository shipments) => _shipments = shipments;

    public async Task<IReadOnlyList<ShipmentResponseDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var list = await _shipments.GetPendingAsync(ct);
        return list.Select(s => new ShipmentResponseDto(
            s.Id, s.OriginAddress, s.DestinationAddress,
            s.CargoDescription, s.WeightKg, s.SuggestedPrice ?? 0m,
            s.Status, s.CreatedAt)).ToList();
    }
}