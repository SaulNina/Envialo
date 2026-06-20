using Envialo.Domain.DTOs.Users;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Ports.IRepositories;

namespace Envialo.Application.UseCases.UserUseCases.Commands;

public sealed class UpdateUserProfileCommand
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork     _uow;

    public UpdateUserProfileCommand(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow   = uow;
    }

    public async Task ExecuteAsync(Guid userId, UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct)
                   ?? throw new DomainException("Usuario no encontrado.");

        user.FullName  = dto.FullName;
        user.Phone     = dto.Phone;
        user.AvatarUrl = dto.AvatarUrl ?? user.AvatarUrl; 
        user.UpdatedAt = DateTime.UtcNow; 

        await _users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
    }
}