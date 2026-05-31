using System;
using System.Collections.Generic;

namespace Envialo.Domain.Entities;

public partial class Trip
{
    public Guid Id { get; set; }

    public Guid ShipmentId { get; set; }

    public Guid DriverId { get; set; }

    public Guid AcceptedOfferId { get; set; }

    public decimal FinalPrice { get; set; }

    public string Currency { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? CancelReason { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual FareOffer AcceptedOffer { get; set; } = null!;

    public virtual User Driver { get; set; } = null!;

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ShipmentRequest Shipment { get; set; } = null!;
}
