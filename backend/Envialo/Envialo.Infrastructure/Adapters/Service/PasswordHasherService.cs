using Envialo.Domain.Ports.IServices;

namespace Envialo.Infrastructure.Adapters.Service;

public sealed class PasswordHasherService : IPasswordHasherService
{
    public string Hash(string plainPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);

    public bool Verify(string plainPassword, string hashedPassword) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
}