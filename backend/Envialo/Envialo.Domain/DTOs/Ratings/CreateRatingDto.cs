namespace Envialo.Domain.DTOs.Ratings;

public record CreateRatingDto(
    Guid TripId, 
    Guid RatedUserId, 
    int Score,        
    string? Comment   
);