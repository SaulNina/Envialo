using Envialo.Domain.DTOs.Users;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.UserUseCases.Queries;

public sealed class GetUserProfileQuery
{
    private readonly IUserRepository _users;

    public GetUserProfileQuery(IUserRepository users)
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