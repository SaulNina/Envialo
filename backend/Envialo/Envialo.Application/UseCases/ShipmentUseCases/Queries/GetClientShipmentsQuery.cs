using Envialo.Domain.DTOs.Shipments;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.ShipmentUseCases.Queries;

public sealed class GetClientShipmentsQuery
{
    private readonly IShipmentRepository _shipmentRepository;

    public GetClientShipmentsQuery(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<IReadOnlyList<ShipmentResponseDto>> ExecuteAsync(Guid clientId, CancellationToken ct)
    {
        var shipments = await _shipmentRepository.GetByClientIdAsync(clientId, ct);

        return shipments.Select(s => new ShipmentResponseDto(
            s.Id,
            s.OriginAddress,
            s.DestinationAddress,
            s.CargoDescription,
            s.WeightKg,
            s.SuggestedPrice ?? 0, 
            s.Status,
            s.CreatedAt
        )).ToList();
    }
}