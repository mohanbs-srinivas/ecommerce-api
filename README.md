# E-commerce API

This is a simple e-commerce API built using .NET 8. The API provides CRUD operations for managing products, orders, order details, customers, and payments. It is designed to serve as a mock backend for e-commerce applications.

## Features

- **Customers**: Create, Read, Update, and Delete customer information.
- **Products**: Manage product details including creation, retrieval, updating, and deletion.
- **Orders**: Handle order processing with CRUD operations.
- **Order Details**: Manage details of each order, including product quantities and prices.
- **Payments**: Manage payment details, including payment type, amount, status, and timestamps.

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

   ```bash
   git clone <repository-url>
   ```

2. Navigate to the project directory:

   ```bash
   cd ecommerce-api
   ```

3. Restore the dependencies:

   ```bash
   dotnet restore
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

### API Documentation

Once the application is running, you can access the Swagger UI for API documentation at:

```
http://localhost:5000/swagger
```

## Mock Data

The application includes a `MockDataInitializer` class that seeds the database with mock data for testing purposes. This can be useful for development and testing without needing a real database setup.

## Payments Feature

The API includes a `PaymentsController` for managing payment details. The following operations are supported:

- **GET** `/api/payments`: Retrieve all payment records.
- **GET** `/api/payments/{id}`: Retrieve a specific payment by ID.
- **POST** `/api/payments`: Create a new payment record.
- **PUT** `/api/payments/{id}`: Update an existing payment record.
- **DELETE** `/api/payments/{id}`: Delete a payment record by ID.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.
