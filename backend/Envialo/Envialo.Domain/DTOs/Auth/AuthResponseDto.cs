namespace Envialo.Domain.DTOs.Auth;

public sealed record AuthResponseDto(
    Guid     UserId,
    string   FullName,
    string   Email,
    string   Role,
    string   AccessToken,
    string   RefreshToken,
    DateTime ExpiresAt
);