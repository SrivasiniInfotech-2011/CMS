namespace CMS.Services.Storage;

public interface ICustomerImageStorage
{
    Task<string> UploadAsync(Stream image, string contentType, CancellationToken cancellationToken);
    Task DeleteAsync(string blobName, CancellationToken cancellationToken);
    Task<CustomerImageDownload?> DownloadAsync(string blobName, CancellationToken cancellationToken);
}

public sealed record CustomerImageDownload(Stream Content, string ContentType);
