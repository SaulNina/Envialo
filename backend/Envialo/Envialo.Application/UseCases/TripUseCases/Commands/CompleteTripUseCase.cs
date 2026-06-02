using Envialo.Application.Abstractions;
using Envialo.Application.Ports;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.TripUseCases.Commands;

public sealed class CompleteTripUseCase
{
    private readonly ITripRepository _trips;
    private readonly IUnitOfWork     _uow;
    
    public CompleteTripUseCase(
        ITripRepository trips,
        IUnitOfWork     uow)
    {
        _trips = trips;
        _uow   = uow;
    }
    public async Task ExecuteAsync(Guid tripId, Guid driverId, CancellationToken ct = default)
    {
        var trip = await _trips.GetByIdAsync(tripId, ct)
                   ?? throw new DomainException($"El viaje con Id '{tripId}' no fue encontrado.");
        
        if (trip.DriverId != driverId)
            throw new UnauthorizedDomainException("No puedes completar un viaje que no te pertenece.");
        
        if (trip.Status != "IN_PROGRESS")
            throw new DomainException($"Solo se puede completar un viaje en progreso. Estado actual: '{trip.Status}'.");
        
        trip.Status      = "COMPLETED"; 
        trip.CompletedAt = DateTime.UtcNow;

        await _trips.UpdateAsync(trip, ct);
        await _uow.SaveChangesAsync(ct);
    }
}