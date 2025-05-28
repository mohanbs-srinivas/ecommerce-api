# E-commerce API

This is a simple e-commerce API built using .NET 8. The API provides CRUD operations for managing products, orders, order details, and customers. It is designed to serve as a mock backend for e-commerce applications.

## Features

- **Customers**: Create, Read, Update, and Delete customer information.
- **Products**: Manage product details including creation, retrieval, updating, and deletion.
- **Orders**: Handle order processing with CRUD operations.
- **Order Details**: Manage details of each order, including product quantities and prices.

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

## Repository Info Endpoints

The API includes endpoints to retrieve information about GitHub repositories:

### GET /api/repositoryinfo/branches
Returns the list of branch names for a specified repository.

**Parameters:**
- `owner` (required): The repository owner (username or organization)
- `repo` (required): The repository name

**Example:**
```
GET /api/repositoryinfo/branches?owner=microsoft&repo=vscode
```

### GET /api/repositoryinfo/commits/count
Returns the total number of commits in the main branch of a specified repository.

**Parameters:**
- `owner` (required): The repository owner (username or organization)
- `repo` (required): The repository name

**Example:**
```
GET /api/repositoryinfo/commits/count?owner=microsoft&repo=vscode
```

### GET /api/repositoryinfo/collaborators
Returns the list of collaborator login names for a specified repository.

**Parameters:**
- `owner` (required): The repository owner (username or organization)
- `repo` (required): The repository name

**Example:**
```
GET /api/repositoryinfo/collaborators?owner=microsoft&repo=vscode
```

### GitHub API Token

For higher rate limits, you can provide a GitHub API token by setting the `GITHUB_API_TOKEN` environment variable. The token will be used as a Bearer token in requests to the GitHub API.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.