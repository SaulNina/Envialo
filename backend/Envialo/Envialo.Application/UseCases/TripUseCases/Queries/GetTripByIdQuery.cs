using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.TripUseCases.Queries;

public sealed class GetTripByIdQuery
{
    private readonly ITripRepository _trips;

    public GetTripByIdQuery(ITripRepository trips) => _trips = trips;

    public async Task<Trip> ExecuteAsync(Guid tripId, CancellationToken ct = default)
        => await _trips.GetByIdAsync(tripId, ct)
           ?? throw new DomainException($"Viaje '{tripId}' no encontrado.");
}