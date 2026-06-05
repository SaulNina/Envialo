using Envialo.Application.DTOs.Users;
using Envialo.Application.Ports;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.UserUseCases.Queries;

public sealed class GetUserProfileUseCase
{
    private readonly IUserRepository _users;

    public GetUserProfileUseCase(IUserRepository users)
    {
        _users = users;
    }

    public async Task<UserProfileDto> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct)
                   ?? throw new DomainException("Usuario no encontrado.");

        return new UserProfileDto(
            user.Id,
            user.Email,
            user.FullName, 
            user.Phone,    
            user.Role,
            user.AvatarUrl,
            user.CreatedAt
        );
    }
}