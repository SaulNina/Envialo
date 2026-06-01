using Envialo.Application.Abstractions;
using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>  ExistsByEmailAsync(string email, CancellationToken ct = default);
}