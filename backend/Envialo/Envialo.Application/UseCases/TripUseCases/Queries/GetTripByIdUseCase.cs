using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.TripUseCases.Queries;

public sealed class GetTripByIdUseCase
{
    private readonly ITripRepository _trips;

    public GetTripByIdUseCase(ITripRepository trips) => _trips = trips;

    public async Task<Trip> ExecuteAsync(Guid tripId, CancellationToken ct = default)
        => await _trips.GetByIdAsync(tripId, ct)
           ?? throw new DomainException($"Viaje '{tripId}' no encontrado.");
}