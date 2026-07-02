using System.Security.Claims;
using Envialo.Domain.DTOs.Offers;
using Envialo.Application.UseCases.FareOfferUseCases.Commands;
using Envialo.Application.UseCases.FareOfferUseCases.Queries;
using Envialo.Domain.Constants;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/fare-offers")] 
[Authorize]
public class FareOffersController : ControllerBase
{
    private readonly CreateFareOfferCommand     _createFareOfferCommand;
    private readonly AcceptFareOfferCommand     _acceptFareOfferCommand;
    private readonly GetOffersByShipmentQuery _getOffersByShipmentQuery;

    public FareOffersController(
        CreateFareOfferCommand     createFareOfferCommand,
        AcceptFareOfferCommand     acceptFareOfferCommand,
        GetOffersByShipmentQuery getOffersByShipmentQuery)
    {
        _createFareOfferCommand     = createFareOfferCommand;
        _acceptFareOfferCommand     = acceptFareOfferCommand;
        _getOffersByShipmentQuery = getOffersByShipmentQuery;
    }

    private Guid GetCurrentUserId()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        
        if (string.IsNullOrEmpty(subClaim))
            throw new UnauthorizedAccessException("Token inválido.");

        return Guid.Parse(subClaim);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Driver)] 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateFareOfferDto dto, CancellationToken ct)
    {
        try
        {
            var driverId = GetCurrentUserId();
            
            await _createFareOfferCommand.ExecuteAsync(dto.ShipmentId, driverId, dto.OfferedPrice, ct);
            
            return StatusCode(StatusCodes.Status201Created, new { Message = "Oferta enviada con éxito." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpGet("{shipmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByShipment(Guid shipmentId, CancellationToken ct)
    {
        try 
        {
            var offers = await _getOffersByShipmentQuery.ExecuteAsync(shipmentId, ct);
            return Ok(offers);
        }
        catch (Exception ex)
        {
            // Ahora sí devolvemos un JSON que Flutter entiende: {"error": "..."}
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}/accept")]
    [Authorize(Roles = UserRoles.Client)] 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Accept(Guid id, CancellationToken ct)
    {
        try
        {
            var clientId = GetCurrentUserId();
            
            await _acceptFareOfferCommand.ExecuteAsync(id, clientId, ct);
            
            return Ok(new { Message = "Oferta aceptada. El viaje ha sido creado." });
        }
        catch (UnauthorizedDomainException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}