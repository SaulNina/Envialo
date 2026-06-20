using System.Security.Claims;
using Envialo.Domain.DTOs.Users;
using Envialo.Application.UseCases.UserUseCases.Commands;
using Envialo.Application.UseCases.UserUseCases.Queries;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize] // Protegido, requiere Token
public class UsersController : ControllerBase
{
    private readonly GetUserProfileUseCase    _getUserProfileUseCase;
    private readonly UpdateUserProfileUseCase _updateUserProfileUseCase;

    public UsersController(
        GetUserProfileUseCase getUserProfileUseCase,
        UpdateUserProfileUseCase updateUserProfileUseCase)
    {
        _getUserProfileUseCase    = getUserProfileUseCase;
        _updateUserProfileUseCase = updateUserProfileUseCase;
    }

    private Guid GetCurrentUserId()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                       ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        
        if (string.IsNullOrEmpty(subClaim))
            throw new UnauthorizedAccessException("Token inválido.");

        return Guid.Parse(subClaim);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            var profile = await _getUserProfileUseCase.ExecuteAsync(userId, ct);
            return Ok(profile);
        }
        catch (DomainException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _updateUserProfileUseCase.ExecuteAsync(userId, dto, ct);
            return Ok(new { Message = "Perfil actualizado correctamente." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}