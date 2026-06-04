namespace Envialo.Application.DTOs.Ratings;

public record RatingResponseDto(
    Guid Id,
    Guid TripId,
    Guid RatedByUserId,
    int Score,
    string? Comment,
    DateTime CreatedAt
);

public record UserRatingSummaryDto(
    double AverageScore,
    int TotalReviews,
    IEnumerable<RatingResponseDto> Reviews
);