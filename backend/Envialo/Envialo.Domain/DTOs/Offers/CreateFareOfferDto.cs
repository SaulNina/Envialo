using System.ComponentModel.DataAnnotations;

namespace Envialo.Domain.DTOs.Offers;

public sealed record CreateFareOfferDto(
    [Required] Guid ShipmentId,
    [Required] [Range(1, 10000)] decimal OfferedPrice,
    [MaxLength(300)] string? DriverNote
);