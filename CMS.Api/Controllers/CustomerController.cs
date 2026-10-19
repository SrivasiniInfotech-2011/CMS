using CMS.Models.Domain;
using CMS.Models.Entities;
using CMS.Services.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController(ICustomerService customerService, ILogger<CustomerController> logger) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CustomerModel>), Description = "Returns a list of all customers")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusCodeResult), Description = "Returns an error status code")]
        public IActionResult GetAllCustomers()
        {
            try
            {
                var customers = customerService.GetAllCustomers();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving customers.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CustomerModel), Description = "Returns the created customer")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(StatusCodeResult), Description = "Returns an error status code")]
        public IActionResult AddCustomer(CustomerModel customerModel)
        {
            try
            {
                var customer = new Customer
                {
                    FirstName = customerModel.FirstName,
                    LastName = customerModel.LastName,
                    MiddleName = customerModel.MiddleName,
                    Dob = customerModel.Dob
                };

                var createdCustomer = customerService.AddCustomer(customer);
                return CreatedAtAction(nameof(AddCustomer), new { id = createdCustomer.Id }, createdCustomer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving customers.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
