using System.Security.Claims;
using Envialo.Domain.DTOs.Shipments;
using Envialo.Application.UseCases.ShipmentUseCases.Commands;
using Envialo.Application.UseCases.ShipmentUseCases.Queries;
using Envialo.Domain.Constants;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class ShipmentsController : ControllerBase
{
    private readonly CreateShipmentCommand      _createShipmentCommand;
    private readonly GetPendingShipmentsQuery _getPendingShipmentsQuery;
    private readonly GetShipmentByIdQuery     _getShipmentByIdQuery;

    public ShipmentsController(
        CreateShipmentCommand      createShipmentCommand,
        GetPendingShipmentsQuery getPendingShipmentsQuery,
        GetShipmentByIdQuery     getShipmentByIdQuery)
    {
        _createShipmentCommand      = createShipmentCommand;
        _getPendingShipmentsQuery = getPendingShipmentsQuery;
        _getShipmentByIdQuery     = getShipmentByIdQuery;
    }

    
    private Guid GetCurrentUserId()
    {

        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                       ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        
        if (string.IsNullOrEmpty(subClaim))
        {
            throw new UnauthorizedAccessException("No se pudo extraer el ID del token del usuario.");
        }

        return Guid.Parse(subClaim);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Client)] 
    [ProducesResponseType(typeof(ShipmentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateShipmentDto dto, CancellationToken ct)
    {
        try
        {
            var clientId = GetCurrentUserId();
            var response = await _createShipmentCommand.ExecuteAsync(dto, clientId, ct);
            
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("pending")]
    [Authorize(Roles = UserRoles.Driver)] //Solo el conductor ve el feed de pendientes
    [ProducesResponseType(typeof(IReadOnlyList<ShipmentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending(CancellationToken ct)
    {
        var list = await _getPendingShipmentsQuery.ExecuteAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShipmentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var shipment = await _getShipmentByIdQuery.ExecuteAsync(id, ct);
            return Ok(shipment);
        }
        catch (ShipmentNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}