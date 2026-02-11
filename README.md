# E-commerce API

This is a simple e-commerce API built using .NET 8. The API provides CRUD operations for managing products, orders, order details, and customers. It is designed to serve as a mock backend for e-commerce applications.

## Features

- **Customers**: Create, Read, Update, and Delete customer information.
- **Products**: Manage product details including creation, retrieval, updating, and deletion.
- **Orders**: Handle order processing with CRUD operations.
- **Order Details**: Manage details of each order, including product quantities and prices.
- **Error Logging**: Automatic error logging to daily text files with detailed exception information.

## Technologies Used

- .NET 8
- C#
- Entity Framework Core
- Swagger for API documentation

## Getting Started

### Prerequisites

- .NET 8 SDK
- A code editor (e.g., Visual Studio Code)

### Installation

1. Clone the repository:
   ```
   git clone <repository-url>
   ```

2. Navigate to the project directory:
   ```
   cd ecommerce-api
   ```

3. Restore the dependencies:
   ```
   dotnet restore
   ```

4. Run the application:
   ```
   dotnet run
   ```

### API Documentation

Once the application is running, you can access the Swagger UI for API documentation at:
```
http://localhost:5000/swagger
```

## Mock Data

The application includes a `MockDataInitializer` class that seeds the database with mock data for testing purposes. This can be useful for development and testing without needing a real database setup.

## Error Logging

The application includes a robust error logging mechanism that automatically logs all unhandled exceptions to daily text files.

### Features

- **Daily Log Rotation**: Log files are created daily with the format `error-log-YYYY-MM-DD.txt` in the `logs` directory.
- **Automatic Logging**: All unhandled exceptions in API controllers are automatically logged via the `ErrorLoggingFilter`.
- **Detailed Information**: Each log entry includes:
  - Timestamp (with milliseconds)
  - Log Level (Error)
  - Exception message
  - Exception type
  - Full stack trace
  - Inner exception details (if present)
- **Thread-Safe**: The logger uses file locking to ensure thread-safe writes.

### Manual Logging

You can also manually log errors by injecting `IFileLogger` into your controllers or services:

```csharp
public class MyController : ControllerBase
{
    private readonly IFileLogger _logger;

    public MyController(IFileLogger logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult MyAction()
    {
        try
        {
            // Your code here
        }
        catch (Exception ex)
        {
            _logger.LogError("Custom error message", ex);
            return StatusCode(500, "An error occurred");
        }
    }
}
```

### Log File Location

Error logs are stored in the `logs` directory in the application root. This directory is automatically created if it doesn't exist and is excluded from version control via `.gitignore`.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.