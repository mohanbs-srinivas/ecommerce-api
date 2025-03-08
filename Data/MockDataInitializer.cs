using System;
using System.Collections.Generic;
using ecommerce_api.Models;
using ecommerce_api.Services;

namespace ecommerce_api.Data
{
    public class MockDataInitializer
    {
        public static void Initialize(CustomerService customerService, OrderService orderService, OrderDetailService orderDetailService, ProductService productService)
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Address = "123 Main St" },
                new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Address = "456 Elm St" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Description = "High performance laptop", Price = 999.99m, Stock = 10 },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 699.99m, Stock = 20 }
            };

            var orders = new List<Order>
            {
                new Order { Id = 1, CustomerId = 1, OrderDate = DateTime.Now, TotalAmount = 999.99m },
                new Order { Id = 2, CustomerId = 2, OrderDate = DateTime.Now, TotalAmount = 699.99m }
            };

            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, Price = 999.99m },
                new OrderDetail { Id = 2, OrderId = 2, ProductId = 2, Quantity = 1, Price = 699.99m }
            };

            foreach (var customer in customers)
            {
                customerService.CreateCustomerAsync(customer).Wait();
            }

            foreach (var product in products)
            {
                productService.AddProductAsync(product).Wait();
            }

            foreach (var order in orders)
            {
                orderService.CreateOrderAsync(order).Wait();
            }

            foreach (var orderDetail in orderDetails)
            {
                orderDetailService.CreateOrderDetailAsync(orderDetail).Wait();
            }
        }
    }
}