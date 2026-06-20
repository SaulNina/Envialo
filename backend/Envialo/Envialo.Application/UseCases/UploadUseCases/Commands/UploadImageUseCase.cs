using Envialo.Domain.Exceptions;
using Envialo.Domain.Ports.IServices;
using Microsoft.AspNetCore.Http;

namespace Envialo.Application.UseCases.UploadUseCases.Commands;

public sealed class UploadImageUseCase
{
    private readonly IStorageService _storage;

    public UploadImageUseCase(IStorageService storage)
    {
        _storage = storage;
    }

    public async Task<string> ExecuteAsync(IFormFile file, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0)
            throw new DomainException("No se ha enviado ningún archivo.");

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new DomainException("Formato no válido. Solo se permiten imágenes (JPG, PNG, WEBP).");

        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        using var stream = file.OpenReadStream();
        var publicUrl = await _storage.UploadImageAsync(stream, uniqueFileName, file.ContentType, ct);

        return publicUrl; 
    }
}