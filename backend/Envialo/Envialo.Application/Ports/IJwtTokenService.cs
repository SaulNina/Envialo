using Envialo.Domain.Entities;

namespace Envialo.Application.Ports;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}