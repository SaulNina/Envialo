using Microsoft.AspNetCore.Http;

namespace Envialo.Domain.DTOs.Uploads;

public class UploadImageDto
{
    public required IFormFile File { get; set; }
}