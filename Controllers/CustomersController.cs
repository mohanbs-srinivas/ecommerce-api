using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.Models;
using ecommerce_api.Services;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(CustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            _logger.LogInformation("Getting all customers");
            var customers = await _customerService.GetAllCustomersAsync();
            _logger.LogInformation("Successfully retrieved {CustomerCount} customers", customers.Count);
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            _logger.LogInformation("Getting customer with ID: {CustomerId}", id);
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Customer with ID: {CustomerId} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved customer with ID: {CustomerId}", id);
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            _logger.LogInformation("Creating new customer with name: {CustomerName}", customer?.Name);
            var createdCustomer = await _customerService.CreateCustomerAsync(customer);
            _logger.LogInformation("Successfully created customer with ID: {CustomerId}", createdCustomer.Id);
            return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, createdCustomer);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, Customer customer)
        {
            _logger.LogInformation("Updating customer with ID: {CustomerId}", id);
            if (id != customer.Id)
            {
                _logger.LogWarning("ID mismatch in update request. URL ID: {UrlId}, Customer ID: {CustomerId}", id, customer.Id);
                return BadRequest();
            }

            var updatedCustomer = await _customerService.UpdateCustomerAsync(customer);
            if (updatedCustomer == null)
            {
                _logger.LogWarning("Customer with ID: {CustomerId} not found for update", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully updated customer with ID: {CustomerId}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);
            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Customer with ID: {CustomerId} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully deleted customer with ID: {CustomerId}", id);
            return NoContent();
        }
    }
}