namespace Envialo.Application.Ports;

public interface IStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);
}