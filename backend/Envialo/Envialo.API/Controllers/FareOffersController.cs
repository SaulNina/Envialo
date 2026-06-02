using System.Security.Claims;
using Envialo.Application.DTOs.Offers;
using Envialo.Application.UseCases.FareOfferUseCases.Commands;
using Envialo.Application.UseCases.FareOfferUseCases.Queries;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/fare-offers")] 
[Authorize]
public class FareOffersController : ControllerBase
{
    private readonly CreateFareOfferUseCase     _createFareOfferUseCase;
    private readonly AcceptFareOfferUseCase     _acceptFareOfferUseCase;
    private readonly GetOffersByShipmentUseCase _getOffersByShipmentUseCase;

    public FareOffersController(
        CreateFareOfferUseCase     createFareOfferUseCase,
        AcceptFareOfferUseCase     acceptFareOfferUseCase,
        GetOffersByShipmentUseCase getOffersByShipmentUseCase)
    {
        _createFareOfferUseCase     = createFareOfferUseCase;
        _acceptFareOfferUseCase     = acceptFareOfferUseCase;
        _getOffersByShipmentUseCase = getOffersByShipmentUseCase;
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
    [Authorize(Roles = "DRIVER")] // Solo conductores ofertan
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateFareOfferDto dto, CancellationToken ct)
    {
        try
        {
            var driverId = GetCurrentUserId();
            
            await _createFareOfferUseCase.ExecuteAsync(dto.ShipmentId, driverId, dto.OfferedPrice, ct);
            
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
        var offers = await _getOffersByShipmentUseCase.ExecuteAsync(shipmentId, ct);
        return Ok(offers);
    }

    [HttpPut("{id:guid}/accept")]
    [Authorize(Roles = "CLIENT")] //Solo el cliente dueño de la carga puede aceptar
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Accept(Guid id, CancellationToken ct)
    {
        try
        {
            var clientId = GetCurrentUserId();
            
            await _acceptFareOfferUseCase.ExecuteAsync(id, clientId, ct);
            
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