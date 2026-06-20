using System.ComponentModel.DataAnnotations;

namespace Envialo.Domain.DTOs.Shipments;

public sealed record CreateShipmentDto(
    [Required] string OriginAddress,
    [Required] string DestinationAddress,
    [Required] string CargoDescription,
    [Range(1, 50000)] decimal WeightKg,
    decimal SuggestedFare
);