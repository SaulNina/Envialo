using Envialo.Application.UseCases.UploadUseCases.Commands;
using Envialo.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envialo.API.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize]
public class UploadsController : ControllerBase
{
    private readonly UploadImageCommand _uploadImageCommand;

    public UploadsController(UploadImageCommand uploadImageCommand)
    {
        _uploadImageCommand = uploadImageCommand;
    }

    [HttpPost("image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage([FromForm] Envialo.Domain.DTOs.Uploads.UploadImageDto dto, CancellationToken ct)
    {
        try
        {
            var publicUrl = await _uploadImageCommand.ExecuteAsync(dto.File, ct);
            
            return Ok(new 
            { 
                Message = "Imagen subida exitosamente.", 
                Url = publicUrl 
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}