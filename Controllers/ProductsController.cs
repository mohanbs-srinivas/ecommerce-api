using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.Models;
using ecommerce_api.Services;
using Microsoft.Extensions.Logging;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            _logger.LogInformation("Retrieving all products at {DateTime}", DateTime.Now);
            var products = await _productService.GetAllProductsAsync();
            _logger.LogInformation("Retrieved {ProductCount} products successfully", products.Count);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            _logger.LogInformation("Retrieving product with ID: {ProductId}", id);
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved product: {ProductName} (ID: {ProductId})", product.Name, product.Id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _logger.LogInformation("Creating new product: {ProductName}", product.Name);
            await _productService.AddProductAsync(product);
            _logger.LogInformation("Successfully created product: {ProductName} with ID: {ProductId}", product.Name, product.Id);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateProductsBulk(IEnumerable<Product> products)
        {
            if (products == null || !products.Any())
            {
                return BadRequest("Product list cannot be empty.");
            }

            await _productService.AddProductsBulkAsync(products);
            return Ok(new { Message = $"{products.Count()} products added successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            await _productService.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}