using System.Security.Claims;
using Envialo.Domain.DTOs.Cancelations;
using Envialo.Application.UseCases.ShipmentUseCases.Commands; 
using Envialo.Application.UseCases.TripUseCases.Commands;     
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/cancelations")]
[Authorize]
public class CancelationsController : ControllerBase
{
    private readonly CancelShipmentCommand _cancelShipmentCommand;
    private readonly CancelTripCommand     _cancelTripCommand;

    public CancelationsController(
        CancelShipmentCommand cancelShipmentCommand,
        CancelTripCommand     cancelTripCommand)
    {
        _cancelShipmentCommand = cancelShipmentCommand;
        _cancelTripCommand     = cancelTripCommand;
    }

    private Guid GetCurrentUserId()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        
        if (string.IsNullOrEmpty(subClaim))
            throw new UnauthorizedAccessException("Token inválido.");

        return Guid.Parse(subClaim);
    }

    [HttpPost("shipments/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelShipment(Guid id, [FromBody] CancelRequestDto dto, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _cancelShipmentCommand.ExecuteAsync(id, userId, dto.Reason, ct);
            return Ok(new { Message = "El flete ha sido cancelado exitosamente." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("trips/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelTrip(Guid id, [FromBody] CancelRequestDto dto, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _cancelTripCommand.ExecuteAsync(id, userId, dto.Reason, ct);
            return Ok(new { Message = "El viaje ha sido cancelado. Se notificará a las partes." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}