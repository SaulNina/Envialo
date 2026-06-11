using System.Security.Claims;
using Envialo.Application.DTOs.Ratings;
using Envialo.Application.UseCases.RatingUseCases.Commands;
using Envialo.Application.UseCases.RatingUseCases.Queries;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/ratings")]
[Authorize]
public class RatingsController : ControllerBase
{
    private readonly CreateRatingUseCase _createRatingUseCase;
    private readonly GetUserRatingsUseCase _getUserRatingsUseCase;

    public RatingsController(CreateRatingUseCase createRatingUseCase, GetUserRatingsUseCase getUserRatingsUseCase)
    {
        _createRatingUseCase = createRatingUseCase;
        _getUserRatingsUseCase = getUserRatingsUseCase;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRating([FromBody] CreateRatingDto dto, CancellationToken ct)
    {
        try
        {
            var raterId = GetCurrentUserId();
            
            var rating = await _createRatingUseCase.ExecuteAsync(
                dto.TripId, 
                raterId, 
                dto.RatedUserId, 
                dto.Score, 
                dto.Comment, 
                ct);
            
            return Ok(new { 
                Message = "Calificación registrada exitosamente.", 
                RatingId = rating.Id 
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(UserRatingSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRatings(Guid userId, CancellationToken ct)
    {
        var result = await _getUserRatingsUseCase.ExecuteAsync(userId, ct);
        return Ok(result);
    }
}