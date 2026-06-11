namespace Envialo.Domain.Constants;

public static class ShipmentStatuses
{
    public const string Open        = "OPEN";
    public const string Negotiating = "NEGOTIATING";
    public const string Accepted    = "ACCEPTED";
    public const string Cancelled   = "CANCELLED";
}

public static class TripStatuses
{
    public const string Confirmed  = "CONFIRMED";
    public const string InProgress = "IN_PROGRESS";
    public const string Completed  = "COMPLETED";
    public const string Cancelled  = "CANCELLED";
}

public static class OfferStatuses
{
    public const string Pending  = "PENDING";
    public const string Accepted = "ACCEPTED";
}

public static class UserStatuses
{
    public const string Active              = "ACTIVE";
    public const string Suspended           = "SUSPENDED";
    public const string Deleted             = "DELETED";
    public const string PendingVerification = "PENDING_VERIFICATION";
}

public static class UserRoles
{
    public const string Client = "CLIENT";
    public const string Driver = "DRIVER";
}

public static class Currencies
{
    public const string Pen = "PEN";
}