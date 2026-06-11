using Envialo.Application.DTOs.Auth;
using Envialo.Application.UseCases.UserUseCases.Commands;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly LoginUseCase        _loginUseCase;

    public AuthController(RegisterUserUseCase registerUseCase, LoginUseCase loginUseCase)
    {
        _registerUserUseCase = registerUseCase;
        _loginUseCase        = loginUseCase;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Actualizamos el tipo de respuesta documentada
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        try
        {
            var userId = await _registerUserUseCase.ExecuteAsync(dto, ct);
            
            return StatusCode(StatusCodes.Status201Created, new { Id = userId, Message = "Usuario registrado." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Ocurrió un error inesperado en el servidor al intentar registrar el usuario." });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        try
        {
            var response = await _loginUseCase.ExecuteAsync(dto, ct);
            return Ok(response);
        }
        catch (UnauthorizedDomainException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}