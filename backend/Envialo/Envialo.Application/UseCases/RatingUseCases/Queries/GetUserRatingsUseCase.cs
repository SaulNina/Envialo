using Envialo.Application.DTOs.Ratings;
using Envialo.Application.Ports;

namespace Envialo.Application.UseCases.RatingUseCases.Queries;

public sealed class GetUserRatingsUseCase
{
    private readonly IRatingRepository _ratings;

    public GetUserRatingsUseCase(IRatingRepository ratings)
    {
        _ratings = ratings;
    }

    public async Task<UserRatingSummaryDto> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        // Obtener todas las reseñas donde este usuario es el calificado
        var ratings = await _ratings.GetByUserIdAsync(userId, ct);
        
        var ratingList = ratings.ToList();

        // Calcular el promedio matemáticamente
        var average = ratingList.Any() ? ratingList.Average(r => r.Score) : 0;
        var total = ratingList.Count;

        // Mapear a DTOs
        var reviews = ratingList.Select(r => new RatingResponseDto(
            r.Id,
            r.TripId,
            r.RatedByUserId,
            r.Score,
            r.Comment,
            r.CreatedAt
        )).OrderByDescending(r => r.CreatedAt); // Los más recientes primero

        return new UserRatingSummaryDto(Math.Round(average, 1), total, reviews);
    }
}