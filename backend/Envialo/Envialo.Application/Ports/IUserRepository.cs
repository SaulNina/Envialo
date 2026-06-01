using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task        AddAsync(User user, CancellationToken ct = default);
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
    Task        AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
    Task        RevokeRefreshTokenAsync(string token, CancellationToken ct = default);
}