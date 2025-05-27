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
            _logger.LogInformation("Retrieved {Count} customers", customers.Count());
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            _logger.LogInformation("Getting customer with id: {CustomerId}", id);
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Customer with id: {CustomerId} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Retrieved customer with id: {CustomerId}", id);
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            _logger.LogInformation("Creating new customer with name: {Name}", customer.Name);
            var createdCustomer = await _customerService.CreateCustomerAsync(customer);
            _logger.LogInformation("Customer created with id: {CustomerId}", createdCustomer.Id);
            return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, createdCustomer);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, Customer customer)
        {
            _logger.LogInformation("Updating customer with id: {CustomerId}", id);
            
            if (id != customer.Id)
            {
                _logger.LogWarning("Update customer failed. ID mismatch: path ID {PathId} vs customer ID {CustomerId}", id, customer.Id);
                return BadRequest();
            }

            var updatedCustomer = await _customerService.UpdateCustomerAsync(customer);
            if (updatedCustomer == null)
            {
                _logger.LogWarning("Customer with id: {CustomerId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Customer with id: {CustomerId} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            _logger.LogInformation("Deleting customer with id: {CustomerId}", id);
            
            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Customer with id: {CustomerId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Customer with id: {CustomerId} deleted successfully", id);
            return NoContent();
        }
    }
}