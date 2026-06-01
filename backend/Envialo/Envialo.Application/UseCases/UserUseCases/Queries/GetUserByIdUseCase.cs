using Envialo.Application.Ports;
using Envialo.Domain.Entities;
using Envialo.Domain.Exceptions;

namespace Envialo.Application.UseCases.UserUseCases.Queries;

public sealed class GetUserByIdUseCase
{
    private readonly IUserRepository _users;

    public GetUserByIdUseCase(IUserRepository users) => _users = users;

    public async Task<User> ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        return await _users.GetByIdAsync(userId, ct)
               ?? throw new DomainException($"Usuario '{userId}' no encontrado.");
    }
}