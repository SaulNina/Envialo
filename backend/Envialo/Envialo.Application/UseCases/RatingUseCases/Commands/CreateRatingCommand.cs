using Envialo.Domain.Constants;
using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.RatingUseCases.Commands;

public sealed class CreateRatingCommand
{
    private readonly IRatingRepository _ratings;
    private readonly ITripRepository   _trips;
    private readonly IUnitOfWork       _uow;

    public CreateRatingCommand(
        IRatingRepository ratings,
        ITripRepository   trips,
        IUnitOfWork       uow)
    {
        _ratings = ratings;
        _trips   = trips;
        _uow     = uow;
    }

    public async Task<Rating> ExecuteAsync(
        Guid tripId, Guid raterId, Guid ratedId,
        int score, string? comment, CancellationToken ct = default)
    {
        var trip = await _trips.GetByIdAsync(tripId, ct)
                   ?? throw new DomainException($"Viaje '{tripId}' no encontrado.");

        if (trip.Status != TripStatuses.Completed)
            throw new DomainException("Solo se pueden calificar viajes completados.");

        if (score is < 1 or > 5)
            throw new DomainException("La calificación debe ser entre 1 y 5.");

        var rating = new Rating
        {
            Id        = Guid.NewGuid(),
            TripId    = tripId,
            RatedByUserId   = raterId,
            RatedUserId = ratedId,
            Score     = (short)score,
            Comment   = comment,
            CreatedAt = DateTime.UtcNow
        };

        await _ratings.AddAsync(rating, ct);
        await _uow.SaveChangesAsync(ct);
        return rating;
    }
}