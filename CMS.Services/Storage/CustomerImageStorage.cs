using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;

namespace CMS.Services.Storage;

public sealed class CustomerImageStorage(
    BlobContainerClient containerClient,
    ILogger<CustomerImageStorage> logger) : ICustomerImageStorage
{
    public async Task<string> UploadAsync(Stream image, string contentType, CancellationToken cancellationToken)
    {
        await containerClient.CreateIfNotExistsAsync(
            publicAccessType: PublicAccessType.None,
            cancellationToken: cancellationToken);

        var extension = contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new ArgumentException("Unsupported customer image content type.", nameof(contentType))
        };
        var blobName = $"{Guid.NewGuid():N}{extension}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(image, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        }, cancellationToken);

        logger.LogInformation("Uploaded customer image blob {BlobName}.", blobName);
        return blobName;
    }

    public async Task DeleteAsync(string blobName, CancellationToken cancellationToken)
    {
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<CustomerImageDownload?> DownloadAsync(string blobName, CancellationToken cancellationToken)
    {
        var blobClient = containerClient.GetBlobClient(blobName);
        if (!(await blobClient.ExistsAsync(cancellationToken)).Value)
        {
            return null;
        }

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return new CustomerImageDownload(
            response.Value.Content,
            response.Value.Details.ContentType ?? "application/octet-stream");
    }
}
