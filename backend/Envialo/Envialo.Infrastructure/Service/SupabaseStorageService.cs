using System.Net.Http.Headers;
using Envialo.Application.Ports;
using Microsoft.Extensions.Configuration;

namespace Envialo.Infrastructure.Service;

public sealed class SupabaseStorageService : IStorageService
{
    private readonly HttpClient _httpClient;
    private readonly string     _supabaseUrl;
    private readonly string     _supabaseKey;
    private readonly string     _bucketName = "shipments"; 

    public SupabaseStorageService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient  = httpClient;
        _supabaseUrl = config["Supabase:Url"] ?? throw new ArgumentNullException("Falta Supabase:Url en appsettings");
        _supabaseKey = config["Supabase:Key"] ?? throw new ArgumentNullException("Falta Supabase:Key en appsettings");
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
    {
        var url = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{fileName}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        request.Headers.Add("apikey", _supabaseKey);

        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        request.Content = content;

        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Fallo al subir a Supabase: {error}");
        }

        return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{fileName}";
    }
}