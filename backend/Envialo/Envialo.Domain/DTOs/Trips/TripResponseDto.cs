namespace Envialo.Domain.DTOs.Trips;

public record TripResponseDto(
    Guid Id,
    Guid ShipmentId,
    string OriginAddress,
    string DestinationAddress,
    decimal FinalPrice,
    string Status, 
    DateTime CreatedAt
);