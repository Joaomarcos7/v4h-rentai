namespace V4H.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct = default);
}
