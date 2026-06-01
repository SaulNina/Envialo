using Envialo.Application.Ports;

namespace Envialo.Application.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository       Users       { get; }
    IShipmentRepository   Shipments   { get; }
    IFareOfferRepository  FareOffers  { get; }
    ITripRepository       Trips       { get; }
    IRatingRepository     Ratings     { get; }

    Task<int> CommitAsync(CancellationToken ct = default);
    Task      RollbackAsync(CancellationToken ct = default);
}