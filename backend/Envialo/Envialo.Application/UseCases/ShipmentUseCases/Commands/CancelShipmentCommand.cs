using Envialo.Domain.Exceptions;
using Envialo.Domain.Constants;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.ShipmentUseCases.Commands;

public sealed class CancelShipmentCommand
{
    private readonly IShipmentRepository _shipments;
    private readonly IUnitOfWork         _uow;

    public CancelShipmentCommand(IShipmentRepository shipments, IUnitOfWork uow)
    {
        _shipments = shipments;
        _uow       = uow;
    }

    public async Task ExecuteAsync(Guid shipmentId, Guid requestingUserId, string reason, CancellationToken ct = default)
    {
        var shipment = await _shipments.GetByIdAsync(shipmentId, ct)
                       ?? throw new ShipmentNotFoundException(shipmentId);

        if (shipment.ClientId != requestingUserId)
            throw new UnauthorizedDomainException("No tienes permiso para cancelar este envío.");

        if (shipment.Status != ShipmentStatuses.Open && shipment.Status != ShipmentStatuses.Negotiating)
            throw new DomainException($"No se puede cancelar un envío en estado '{shipment.Status}'.");

        shipment.Status = ShipmentStatuses.Cancelled;
        shipment.CancelReason = reason; 

        await _shipments.UpdateAsync(shipment, ct);
        await _uow.SaveChangesAsync(ct);
    }
}