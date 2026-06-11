using Envialo.Application.Abstractions;
using Envialo.Application.Ports;
using Envialo.Domain.Constants;
using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.TripUseCases.Commands;

public sealed class StartTripUseCase
{
    private readonly ITripRepository     _trips;
    private readonly IUnitOfWork         _uow;

    public StartTripUseCase(
        ITripRepository     trips,
        IUnitOfWork         uow)
    {
        _trips     = trips;
        _uow       = uow;
    }

    public async Task<Trip> ExecuteAsync(Guid tripId, Guid driverId, CancellationToken ct = default)
    {
        var trip = await _trips.GetByIdAsync(tripId, ct)
                   ?? throw new DomainException($"El viaje con Id '{tripId}' no fue encontrado.");

        if (trip.DriverId != driverId)
            throw new UnauthorizedDomainException("No puedes iniciar un viaje que no te pertenece.");
        
        if (trip.Status != TripStatuses.Confirmed)
            throw new DomainException($"El viaje no se puede iniciar porque está en estado '{trip.Status}'.");
        
        trip.Status    = TripStatuses.InProgress;
        trip.StartedAt = DateTime.UtcNow;
        
        await _trips.UpdateAsync(trip, ct);
        await _uow.SaveChangesAsync(ct);
        
        return trip;
    }
}