using Envialo.Domain.DTOs.Auth;
using Envialo.Domain.Ports.IRepositories;
using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Constants;
using Envialo.Domain.Ports.IServices;

namespace Envialo.Application.UseCases.UserUseCases.Commands;

public sealed class RegisterUserUseCase
{
    private readonly IUserRepository      _users;
    private readonly IPasswordHasherService _hasher;
    private readonly IJwtTokenService     _jwt;
    private readonly IUnitOfWork          _uow;

    public RegisterUserUseCase(
        IUserRepository      users,
        IPasswordHasherService hasher,
        IJwtTokenService     jwt,
        IUnitOfWork          uow)
    {
        _users  = users;
        _hasher = hasher;
        _jwt    = jwt;
        _uow    = uow;
    }

    public async Task<AuthResponseDto> ExecuteAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await _users.ExistsByEmailAsync(dto.Email.ToLower(), ct))
            throw new DomainException($"El email '{dto.Email}' ya está registrado.");

        var user = new User
        {
            Id           = Guid.NewGuid(),
            FullName     = dto.FullName.Trim(),
            Email        = dto.Email.ToLower().Trim(),
            PasswordHash = _hasher.Hash(dto.Password),
            Phone        = dto.Phone,
            Role         = dto.Role,
            Status     = UserStatuses.Active,
            CreatedAt    = DateTime.UtcNow
        };

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return BuildResponse(user);
    }

    private AuthResponseDto BuildResponse(User user) => new(
        user.Id,
        user.FullName,
        user.Email,
        user.Role,
        _jwt.GenerateAccessToken(user),
        _jwt.GenerateRefreshToken(),
        DateTime.UtcNow.AddMinutes(60)
    );
}