using Envialo.Domain.Entities;

namespace Envialo.Domain.Ports.IServices;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}