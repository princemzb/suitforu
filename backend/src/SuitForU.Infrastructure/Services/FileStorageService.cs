using SuitForU.Application.Interfaces;

namespace SuitForU.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public FileStorageService()
    {
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_storagePath, uniqueFileName);

        using var fileStreamOut = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOut, cancellationToken);

        return $"/uploads/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var fileName = fileUrl.Split('/').Last();
        var filePath = Path.Combine(_storagePath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
