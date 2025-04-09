# AzureDevOpsService Documentation

## Overview

The `AzureDevOpsService` class is a C# API service designed to interact with the Azure DevOps REST API. It supports the following HTTP methods:
- **GET**
- **POST**
- **PATCH**

This class simplifies communication with Azure DevOps by handling authentication and providing methods for common operations.

---

## Features

- **GET Requests**: Retrieve data from Azure DevOps endpoints.
- **POST Requests**: Send data to Azure DevOps endpoints.
- **PATCH Requests**: Update data on Azure DevOps endpoints.
- **Authentication**: Uses a Personal Access Token (PAT) for secure communication.

---

## Class Definition

```csharp
public class AzureDevOpsService : IDisposable
```

### Constructor

```csharp
public AzureDevOpsService(string organization, string project, string personalAccessToken)
```

- **Parameters**:
    - `organization`: The name of your Azure DevOps organization.
    - `project`: The name of your Azure DevOps project.
    - `personalAccessToken`: A PAT for authenticating requests.

---

## Methods

### `GetAsync`

```csharp
public async Task<string> GetAsync(string endpoint)
```

- **Description**: Sends a GET request to the specified endpoint.
- **Parameters**:
    - `endpoint`: The relative URL of the Azure DevOps API endpoint.
- **Returns**: The response content as a string.
- **Exceptions**: Throws `HttpRequestException` for unsuccessful responses.

---

### `PostAsync`

```csharp
public async Task<string> PostAsync(string endpoint, object data)
```

- **Description**: Sends a POST request with JSON-serialized data.
- **Parameters**:
    - `endpoint`: The relative URL of the Azure DevOps API endpoint.
    - `data`: The object to be serialized into JSON and sent as the request body.
- **Returns**: The response content as a string.
- **Exceptions**: Throws `HttpRequestException` for unsuccessful responses.

---

### `PatchAsync`

```csharp
public async Task<string> PatchAsync(string endpoint, object data)
```

- **Description**: Sends a PATCH request with JSON-serialized data.
- **Parameters**:
    - `endpoint`: The relative URL of the Azure DevOps API endpoint.
    - `data`: The object to be serialized into JSON and sent as the request body.
- **Returns**: The response content as a string.
- **Exceptions**: Throws `HttpRequestException` for unsuccessful responses.

---

### `Dispose`

```csharp
public void Dispose()
```

- **Description**: Disposes of the `HttpClient` instance to release resources.

---

## Example Usage

```csharp
var service = new AzureDevOpsService("myOrg", "myProject", "myPersonalAccessToken");

try
{
        // GET example
        var getResult = await service.GetAsync("workitems?ids=1");
        Console.WriteLine(getResult);

        // POST example
        var postData = new { title = "New Work Item", description = "Work item description" };
        var postResult = await service.PostAsync("workitems", postData);
        Console.WriteLine(postResult);

        // PATCH example
        var patchData = new { title = "Updated Work Item" };
        var patchResult = await service.PatchAsync("workitems/1", patchData);
        Console.WriteLine(patchResult);
}
finally
{
        service.Dispose();
}
```

---

## Dependencies

- **Newtonsoft.Json**: Used for JSON serialization and deserialization.
- **System.Net.Http**: Provides HTTP client functionality.

---

## Notes

- Ensure your Personal Access Token (PAT) has the necessary permissions for the operations you intend to perform.
- Always dispose of the `AzureDevOpsService` instance to release resources.

---

## License

This code is provided as-is without any warranty. Ensure compliance with your organization's policies when using this code.
