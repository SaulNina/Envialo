using Envialo.Domain.Ports.IRepositories;
using Envialo.Domain.Ports.IServices;
using Envialo.Domain.DTOs.Auth;
using Envialo.Domain.Exceptions;
using Envialo.Domain.Constants;


namespace Envialo.Application.UseCases.UserUseCases.Commands;

public sealed class LoginUseCase
{
    private readonly IUserRepository        _users;
    private readonly IPasswordHasherService _hasher;
    private readonly IJwtTokenService       _jwt;
    private readonly IUnitOfWork            _uow;

    public LoginUseCase(
        IUserRepository        users,
        IPasswordHasherService hasher,
        IJwtTokenService       jwt,
        IUnitOfWork            uow)
    {
        _users  = users;
        _hasher = hasher;
        _jwt    = jwt;
        _uow    = uow;
    }

    public async Task<AuthResponseDto> ExecuteAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(dto.Email.ToLower(), ct)
                   ?? throw new UnauthorizedDomainException("Credenciales inválidas.");

        if (user.Status is UserStatuses.Suspended or UserStatuses.Deleted or UserStatuses.PendingVerification)
            throw new UnauthorizedDomainException($"La cuenta no está activa. Estado actual: {user.Status}");

        if (!_hasher.Verify(dto.Password, user.PasswordHash!))
            throw new UnauthorizedDomainException("Credenciales inválidas.");

        return new AuthResponseDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            _jwt.GenerateAccessToken(user),
            _jwt.GenerateRefreshToken(), 
            DateTime.UtcNow.AddMinutes(60)
        );
    }
}