using System;
using System.Collections.Generic;

namespace Envialo.Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual DriverProfile? DriverProfile { get; set; }

    public virtual ICollection<FareOffer> FareOffers { get; set; } = new List<FareOffer>();

    public virtual ICollection<Rating> RatingRatedByUsers { get; set; } = new List<Rating>();

    public virtual ICollection<Rating> RatingRatedUsers { get; set; } = new List<Rating>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<ShipmentRequest> ShipmentRequests { get; set; } = new List<ShipmentRequest>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
