namespace Envialo.Domain.Ports.IServices;

public interface IPasswordHasherService
{
    string Hash(string plainPassword);
    bool   Verify(string plainPassword, string hashedPassword);
}