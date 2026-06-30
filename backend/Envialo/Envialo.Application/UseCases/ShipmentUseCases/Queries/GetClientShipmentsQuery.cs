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
        // Usamos el método que ya tienes definido en tu IShipmentRepository
        var shipments = await _shipmentRepository.GetByClientIdAsync(clientId, ct);

        // Mapeamos de Entidad a DTO para no exponer la base de datos
        return shipments.Select(s => new ShipmentResponseDto(
            s.Id,
            s.OriginAddress,
            s.DestinationAddress,
            s.CargoDescription,
            s.WeightKg,
            s.SuggestedPrice ?? 0, // Mapeamos SuggestedPrice a SuggestedFare del DTO
            s.Status,
            s.CreatedAt
        )).ToList();
    }
}