using CMS.Models.Domain;
using CMS.Models.Entities;
using CMS.Models.Requests;
using CMS.Services.Customers;
using CMS.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(
        ICustomerService customerService,
        ICustomerImageStorage imageStorage,
        ILogger<CustomerController> logger) : ControllerBase
    {
        private const long MaxImageSize = 5 * 1024 * 1024;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CustomerModel>), Description = "Returns a list of all customers")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusCodeResult), Description = "Returns an error status code")]
        public IActionResult GetAllCustomers()
        {
            try
            {
                return Ok(customerService.GetAllCustomers());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving customers.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerModel), Description = "Returns a customer")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusCodeResult), Description = "Returns an error status code")]
        public IActionResult GetCustomer([FromRoute] int id)
        {
            try
            {
                var customer = customerService.SearchCustomer(customer => customer.Id == id);
                if (customer == null)
                {
                    return NotFound();
                }

                return Ok(new CustomerModel
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    MiddleName = customer.MiddleName,
                    Dob = customer.Dob,
                    ImageUrl = customer.ImageBlobName == null ? null : $"/api/Customer/{customer.Id}/image"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the customer.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}/image")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult), Description = "Returns the customer's image")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodeResult), Description = "Customer or image was not found")]
        public async Task<IActionResult> GetCustomerImage([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                var customer = customerService.SearchCustomer(item => item.Id == id);
                if (customer?.ImageBlobName is null)
                {
                    return NotFound();
                }

                var image = await imageStorage.DownloadAsync(customer.ImageBlobName, cancellationToken);
                if (image is null)
                {
                    return NotFound();
                }

                return File(image.Content, image.ContentType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the customer image for customer {CustomerId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerModel), Description = "Returns the updated customer")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(StatusCodeResult), Description = "Returns a not found status code")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusCodeResult), Description = "Returns an error status code")]
        public IActionResult UpdateCustomer([FromRoute] int id, [FromBody] CustomerModel customerModel)
        {
            try
            {
                var existingCustomer = customerService.SearchCustomer(customer => customer.Id == id);
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                existingCustomer.FirstName = customerModel.FirstName;
                existingCustomer.LastName = customerModel.LastName;
                existingCustomer.MiddleName = customerModel.MiddleName;
                existingCustomer.Dob = customerModel.Dob;
                return Ok(customerService.UpdateCustomer(existingCustomer));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating the customer.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxImageSize + 64 * 1024)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CustomerModel), Description = "Returns the created customer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails), Description = "Customer details or image are invalid")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusCodeResult), Description = "Returns an error status code")]
        public async Task<IActionResult> AddCustomer(
            [FromForm] CreateCustomerRequest customerModel,
            IFormFile? customerImage,
            CancellationToken cancellationToken)
        {
            if (customerImage is null || customerImage.Length == 0)
            {
                return BadRequest("A customer image is required.");
            }

            if (customerImage.Length > MaxImageSize)
            {
                return BadRequest("Customer images must be 5 MB or smaller.");
            }

            var contentType = customerImage.ContentType.ToLowerInvariant();
            await using var imageStream = customerImage.OpenReadStream();
            if (!await IsSupportedImageAsync(imageStream, contentType, cancellationToken))
            {
                return BadRequest("Only valid JPEG, PNG, and WebP images are supported.");
            }

            string? blobName = null;
            try
            {
                blobName = await imageStorage.UploadAsync(imageStream, contentType, cancellationToken);
                var customer = new Customer
                {
                    FirstName = customerModel.FirstName,
                    LastName = customerModel.LastName,
                    MiddleName = customerModel.MiddleName,
                    Dob = customerModel.Dob,
                    ImageBlobName = blobName
                };

                try
                {
                    var createdCustomer = customerService.AddCustomer(customer);
                    return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, createdCustomer);
                }
                catch
                {
                    try
                    {
                        await imageStorage.DeleteAsync(blobName, CancellationToken.None);
                    }
                    catch (Exception cleanupException)
                    {
                        logger.LogError(cleanupException, "Failed to remove orphaned customer image blob {BlobName}.", blobName);
                    }

                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a customer and storing its image.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        private static async Task<bool> IsSupportedImageAsync(Stream stream, string contentType, CancellationToken cancellationToken)
        {
            if (!stream.CanSeek)
            {
                return false;
            }

            var header = new byte[12];
            var headerLength = 0;
            while (headerLength < header.Length)
            {
                var bytesRead = await stream.ReadAsync(header.AsMemory(headerLength), cancellationToken);
                if (bytesRead == 0)
                {
                    break;
                }

                headerLength += bytesRead;
            }

            stream.Position = 0;
            return contentType switch
            {
                "image/jpeg" => headerLength >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                "image/png" => headerLength >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                "image/webp" => headerLength >= 12 && header.AsSpan(0, 4).SequenceEqual("RIFF"u8) && header.AsSpan(8, 4).SequenceEqual("WEBP"u8),
                _ => false
            };
        }
    }
}
