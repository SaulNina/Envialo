using System;
using System.Collections.Generic;

namespace Envialo.Domain.Entities;

public partial class Rating
{
    public Guid Id { get; set; }

    public Guid TripId { get; set; }

    public Guid RatedByUserId { get; set; }

    public Guid RatedUserId { get; set; }

    public short Score { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User RatedByUser { get; set; } = null!;

    public virtual User RatedUser { get; set; } = null!;

    public virtual Trip Trip { get; set; } = null!;
}
