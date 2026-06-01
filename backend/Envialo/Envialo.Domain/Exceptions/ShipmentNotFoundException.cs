namespace Envialo.Domain.Exceptions;

public class ShipmentNotFoundException : DomainException
{
    public ShipmentNotFoundException(Guid id)
        : base($"El envío con Id '{id}' no fue encontrado.") { }
}