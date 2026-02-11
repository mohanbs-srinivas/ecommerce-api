using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ecommerce_api.Logging
{
    /// <summary>
    /// Middleware to handle exceptions globally and log errors
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFileLogger _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, IFileLogger logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the error
                var errorMessage = $"Unhandled exception occurred while processing request: {context.Request.Method} {context.Request.Path}";
                _logger.LogError(errorMessage, ex);

                // In development, rethrow to let developer exception page handle it
                if (_environment.IsDevelopment())
                {
                    throw;
                }

                // In production, handle the exception gracefully
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                error = "An error occurred while processing your request.",
                message = exception.Message,
                statusCode = context.Response.StatusCode
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
