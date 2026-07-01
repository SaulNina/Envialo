using Envialo.Domain.DTOs.Trips;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.TripUseCases.Queries;

public sealed class GetDriverTripsQuery
{
    private readonly ITripRepository _tripRepository;

    public GetDriverTripsQuery(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<IReadOnlyList<TripResponseDto>> ExecuteAsync(Guid driverId, CancellationToken ct)
    {
        var trips = await _tripRepository.GetByDriverIdAsync(driverId, ct);

        return trips.Select(t => new TripResponseDto(
            t.Id,
            t.ShipmentId,
            t.Shipment.OriginAddress,      
            t.Shipment.DestinationAddress, 
            t.FinalPrice,
            t.Status,
            t.CreatedAt
        )).ToList();
    }
}