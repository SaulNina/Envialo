namespace Envialo.Domain.Ports.IServices;

public interface IStorageService 
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);
}