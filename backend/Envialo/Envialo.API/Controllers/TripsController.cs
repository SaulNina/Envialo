using System.Security.Claims;
using Envialo.Application.UseCases.TripUseCases.Commands;
using Envialo.Application.UseCases.TripUseCases.Queries;
using Envialo.Domain.Constants;
using Envialo.Domain.DTOs.Trips;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/trips")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly StartTripCommand    _startTripCommand;
    private readonly CompleteTripCommand _completeTripCommand;
    private readonly GetTripByIdQuery  _getTripByIdQuery;
    private readonly GetDriverTripsQuery _getDriverTripsQuery;

    public TripsController(
        StartTripCommand    startTripCommand,
        CompleteTripCommand completeTripCommand,
        GetTripByIdQuery  getTripByIdQuery,
        GetDriverTripsQuery getDriverTripsQuery)
    {
        _startTripCommand    = startTripCommand;
        _completeTripCommand = completeTripCommand;
        _getTripByIdQuery  = getTripByIdQuery;
        _getDriverTripsQuery = getDriverTripsQuery;
    }

    private Guid GetCurrentUserId()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        
        if (string.IsNullOrEmpty(subClaim))
            throw new UnauthorizedAccessException("Token inválido.");

        return Guid.Parse(subClaim);
    }
    
    [HttpGet("driver")]
    [Authorize(Roles = UserRoles.Driver)]
    [ProducesResponseType(typeof(IReadOnlyList<TripResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDriverTrips(CancellationToken ct)
    {
        try
        {
            var driverId = GetCurrentUserId();
            var list = await _getDriverTripsQuery.ExecuteAsync(driverId, ct);
            return Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var trip = await _getTripByIdQuery.ExecuteAsync(id, ct);
            return Ok(trip);
        }
        catch (DomainException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/start")]
    [Authorize(Roles = UserRoles.Driver)] //Solo el conductor que aceptó el viaje puede iniciarlo
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        try
        {
            var driverId = GetCurrentUserId();
            // Ejecutamos el caso de uso pasando el ID del viaje y el ID del conductor para validar que sea él
            await _startTripCommand.ExecuteAsync(id, driverId, ct);
            
            return Ok(new { Message = "Viaje iniciado. ¡Buen camino!" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = UserRoles.Driver)] // Solo el conductor puede finalizarlo
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        try
        {
            var driverId = GetCurrentUserId();
            await _completeTripCommand.ExecuteAsync(id, driverId, ct);
            
            return Ok(new { Message = "Viaje completado exitosamente. Carga entregada." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}