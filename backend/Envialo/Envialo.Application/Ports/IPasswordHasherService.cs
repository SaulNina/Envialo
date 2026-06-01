namespace Envialo.Application.Ports;

public interface IPasswordHasherService
{
    string Hash(string plainPassword);
    bool   Verify(string plainPassword, string hashedPassword);
}