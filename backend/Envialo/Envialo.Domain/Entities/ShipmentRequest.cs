using System;
using System.Collections.Generic;

namespace Envialo.Domain.Entities;

public partial class ShipmentRequest
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    public string OriginAddress { get; set; } = null!;

    public string? OriginReference { get; set; }

    public string DestinationAddress { get; set; } = null!;

    public string? DestReference { get; set; }

    public string CargoDescription { get; set; } = null!;

    public decimal WeightKg { get; set; }

    public decimal? WidthCm { get; set; }

    public decimal? HeightCm { get; set; }

    public decimal? DepthCm { get; set; }

    public bool RequiresRefrigeration { get; set; }

    public bool Fragile { get; set; }

    public string? PhotoUrl { get; set; }

    public decimal? SuggestedPrice { get; set; }

    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? CancelReason { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User Client { get; set; } = null!;

    public virtual ICollection<FareOffer> FareOffers { get; set; } = new List<FareOffer>();

    public virtual Trip? Trip { get; set; }
}
