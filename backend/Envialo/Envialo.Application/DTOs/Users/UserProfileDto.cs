namespace Envialo.Application.DTOs.Users;

public record UserProfileDto(
    Guid Id,
    string Email,
    string FullName, 
    string Phone,    
    string Role,
    string? AvatarUrl,
    DateTime CreatedAt
);

public record UpdateUserDto(
    string FullName, 
    string Phone,
    string? AvatarUrl
);