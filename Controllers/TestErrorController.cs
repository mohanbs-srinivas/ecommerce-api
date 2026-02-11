using Microsoft.AspNetCore.Mvc;
using ecommerce_api.Logging;

namespace ecommerce_api.Controllers
{
    /// <summary>
    /// Test controller to demonstrate error logging functionality
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestErrorController : ControllerBase
    {
        private readonly IFileLogger _logger;

        public TestErrorController(IFileLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Test endpoint that throws an exception to demonstrate error logging
        /// </summary>
        [HttpGet("trigger-error")]
        public IActionResult TriggerError()
        {
            throw new InvalidOperationException("This is a test error to demonstrate the error logging mechanism.");
        }

        /// <summary>
        /// Test endpoint that manually logs an error without throwing
        /// </summary>
        [HttpGet("log-error")]
        public IActionResult LogError()
        {
            try
            {
                // Simulate some operation that might fail
                int divisor = 0;
                var result = 10 / divisor; // This will throw DivideByZeroException
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manual error logging test: Division by zero", ex);
                return StatusCode(500, new { message = "Error logged successfully", error = ex.Message });
            }
        }
    }
}
