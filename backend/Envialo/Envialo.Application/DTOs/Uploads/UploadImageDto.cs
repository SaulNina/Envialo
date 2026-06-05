using Microsoft.AspNetCore.Http;

namespace Envialo.Application.DTOs.Uploads;

public class UploadImageDto
{
    public required IFormFile File { get; set; }
}