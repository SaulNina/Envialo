using System;
using System.Collections.Generic;

namespace Envialo.Domain.Entities;

public partial class FareOffer
{
    public Guid Id { get; set; }

    public Guid ShipmentId { get; set; }

    public Guid DriverId { get; set; }

    public decimal OfferedPrice { get; set; }

    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? DriverNote { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User Driver { get; set; } = null!;

    public virtual ShipmentRequest Shipment { get; set; } = null!;

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
