using Envialo.Application.Abstractions;
using Envialo.Application.DTOs.Auth;
using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.UserUseCases.Commands;

public sealed class LoginUseCase
{
    private readonly IUserRepository      _users;
    private readonly IPasswordHasherService _hasher;
    private readonly IJwtTokenService     _jwt;
    private readonly IUnitOfWork          _uow;

    public LoginUseCase(
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

    public async Task<AuthResponseDto> ExecuteAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(dto.Email.ToLower(), ct)
                   ?? throw new UnauthorizedDomainException("Credenciales inválidas.");

        if (!user.IsActive)
            throw new UnauthorizedDomainException("La cuenta está desactivada.");

        if (!_hasher.Verify(dto.Password, user.PasswordHash!))
            throw new UnauthorizedDomainException("Credenciales inválidas.");

        await _uow.SaveChangesAsync(ct); // para persistir refresh token si se agrega

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