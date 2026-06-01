using Envialo.Application.Abstractions;
using Envialo.Application.Ports;

namespace Envialo.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public IUserRepository      Users      { get; }
    public IShipmentRepository  Shipments  { get; }
    public IFareOfferRepository FareOffers { get; }
    public ITripRepository      Trips      { get; }
    public IRatingRepository    Ratings    { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db        = db;
        Users      = new UserRepository(db);
        Shipments  = new ShipmentRepository(db);
        FareOffers = new FareOfferRepository(db);
        Trips      = new TripRepository(db);
        Ratings    = new RatingRepository(db);
    }

    public Task<int> CommitAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        // EF Core: revertir todos los cambios trackeados
        foreach (var entry in _db.ChangeTracker.Entries())
            await entry.ReloadAsync(ct);
    }

    public async ValueTask DisposeAsync()
        => await _db.DisposeAsync();
}