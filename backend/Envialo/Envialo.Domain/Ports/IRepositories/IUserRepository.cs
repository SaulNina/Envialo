using Envialo.Domain.Entities;
using Envialo.Domain.Ports.IServices;

namespace Envialo.Domain.Ports.IRepositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>  ExistsByEmailAsync(string email, CancellationToken ct = default);
}