using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.Models;

namespace ecommerce_api.Services
{
    public class CustomerService
    {
        private readonly List<Customer> _customers;

        public CustomerService()
        {
            _customers = new List<Customer>();
        }

        public Task<List<Customer>> GetAllCustomersAsync()
        {
            return Task.FromResult(_customers);
        }

        public Task<Customer?> GetCustomerByIdAsync(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(customer);
        }

        public Task<Customer> CreateCustomerAsync(Customer customer)
        {
            customer.Id = _customers.Count > 0 ? _customers.Max(c => c.Id) + 1 : 1;
            _customers.Add(customer);
            return Task.FromResult(customer);
        }

        public Task<Customer?> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                existingCustomer.Address = customer.Address;
            }
            return Task.FromResult(existingCustomer);
        }

        public Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}