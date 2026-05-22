using Microsoft.Extensions.Configuration;
using V4H.Application.Common.Interfaces;

namespace V4H.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IConfiguration config)
    {
        _basePath = config["Storage:BasePath"] ?? "uploads";
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct = default)
    {
        var unique = $"{Guid.NewGuid()}_{fileName}";
        var path = Path.Combine(_basePath, unique);
        await using var fs = File.Create(path);
        await content.CopyToAsync(fs, ct);
        return path;
    }
}
