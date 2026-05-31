using System;
using System.Collections.Generic;

namespace Envialo.Domain.Entities;

public partial class DriverProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public DateOnly LicenseExpiry { get; set; }

    public string VehicleType { get; set; } = null!;

    public string VehiclePlate { get; set; } = null!;

    public string? VehicleBrand { get; set; }

    public string? VehicleModel { get; set; }

    public short? VehicleYear { get; set; }

    public decimal PayloadCapacityKg { get; set; }

    public decimal? VolumeCapacityM3 { get; set; }

    public string Status { get; set; } = null!;

    public decimal AvgRating { get; set; }

    public int TotalTrips { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
