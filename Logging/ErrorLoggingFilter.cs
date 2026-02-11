using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ecommerce_api.Logging
{
    /// <summary>
    /// Exception filter to log all unhandled exceptions
    /// </summary>
    public class ErrorLoggingFilter : IExceptionFilter
    {
        private readonly IFileLogger _logger;

        public ErrorLoggingFilter(IFileLogger logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception
            var errorMessage = $"Unhandled exception in {context.ActionDescriptor.DisplayName}";
            _logger.LogError(errorMessage, context.Exception);
            
            // Don't mark as handled - let other handlers process it
        }
    }
}
