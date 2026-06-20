namespace Envialo.Domain.DTOs.Shipments;

public sealed record ShipmentResponseDto(
    Guid    Id,
    string  OriginAddress,
    string  DestinationAddress,
    string  CargoDescription,
    decimal WeightKg,
    decimal SuggestedFare,
    string  Status,
    DateTime CreatedAt
);