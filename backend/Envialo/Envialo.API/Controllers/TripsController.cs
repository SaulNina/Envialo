using System.Security.Claims;
using Envialo.Application.UseCases.TripUseCases.Commands;
using Envialo.Application.UseCases.TripUseCases.Queries;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/trips")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly StartTripUseCase    _startTripUseCase;
    private readonly CompleteTripUseCase _completeTripUseCase;
    private readonly GetTripByIdUseCase  _getTripByIdUseCase;

    public TripsController(
        StartTripUseCase    startTripUseCase,
        CompleteTripUseCase completeTripUseCase,
        GetTripByIdUseCase  getTripByIdUseCase)
    {
        _startTripUseCase    = startTripUseCase;
        _completeTripUseCase = completeTripUseCase;
        _getTripByIdUseCase  = getTripByIdUseCase;
    }

    private Guid GetCurrentUserId()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        
        if (string.IsNullOrEmpty(subClaim))
            throw new UnauthorizedAccessException("Token inválido.");

        return Guid.Parse(subClaim);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var trip = await _getTripByIdUseCase.ExecuteAsync(id, ct);
            return Ok(trip);
        }
        catch (DomainException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/start")]
    [Authorize(Roles = "DRIVER")] // 🔒 Solo el conductor que aceptó el viaje puede iniciarlo
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        try
        {
            var driverId = GetCurrentUserId();
            // Ejecutamos el caso de uso pasando el ID del viaje y el ID del conductor para validar que sea él
            await _startTripUseCase.ExecuteAsync(id, driverId, ct);
            
            return Ok(new { Message = "Viaje iniciado. ¡Buen camino!" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "DRIVER")] // 🔒 Solo el conductor puede finalizarlo
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        try
        {
            var driverId = GetCurrentUserId();
            await _completeTripUseCase.ExecuteAsync(id, driverId, ct);
            
            return Ok(new { Message = "Viaje completado exitosamente. Carga entregada." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}