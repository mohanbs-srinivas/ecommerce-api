# PaymentController Documentation

## Overview
The `PaymentController` is a RESTful API controller in the `ecommerce-api` project. It provides endpoints to manage payment records, including creating, retrieving, updating, and deleting payments.

### Namespace
```csharp
namespace ecommerce_api.Controllers
```

### Route
The controller is accessible via the route:
```
api/Payment
```

## Endpoints

### 1. **GET: `api/Payment`**
Retrieves all payment records.

- **Method**: `HttpGet`
- **Response**: 
    - `200 OK`: Returns a list of all payments.

---

### 2. **GET: `api/Payment/{id}`**
Retrieves a specific payment by its unique identifier.

- **Method**: `HttpGet`
- **Parameters**:
    - `id` (int): The unique identifier of the payment.
- **Response**:
    - `200 OK`: Returns the payment details if found.
    - `404 Not Found`: Returns an error message if the payment does not exist.

---

### 3. **POST: `api/Payment`**
Creates a new payment record.

- **Method**: `HttpPost`
- **Request Body**:
    - A `Payment` object containing the payment details.
- **Response**:
    - `201 Created`: Returns the created payment with its unique identifier.
    - `400 Bad Request`: Returns an error message if the input data is invalid.

---

### 4. **PUT: `api/Payment/{id}`**
Updates an existing payment record.

- **Method**: `HttpPut`
- **Parameters**:
    - `id` (int): The unique identifier of the payment to update.
- **Request Body**:
    - A `Payment` object containing the updated details.
- **Response**:
    - `200 OK`: Returns the updated payment details.
    - `404 Not Found`: Returns an error message if the payment does not exist.

---

### 5. **DELETE: `api/Payment/{id}`**
Deletes a payment record.

- **Method**: `HttpDelete`
- **Parameters**:
    - `id` (int): The unique identifier of the payment to delete.
- **Response**:
    - `204 No Content`: Indicates the payment was successfully deleted.
    - `404 Not Found`: Returns an error message if the payment does not exist.

---

## Models

### Payment
The `Payment` model represents a payment record with the following properties:
- `PaymentID` (int): The unique identifier of the payment.
- `PaymentType` (string): The type of payment (e.g., Credit Card, PayPal).
- `Amount` (decimal): The amount of the payment.
- `DateTime` (DateTime): The date and time of the payment.
- `Status` (string): The status of the payment (e.g., Completed, Pending).

---

## Example Usage

### Create a Payment
**Request**:
```json
POST /api/Payment
{
    "PaymentType": "Credit Card",
    "Amount": 100.50,
    "DateTime": "2023-10-01T12:00:00",
    "Status": "Completed"
}
```

**Response**:
```json
201 Created
{
    "PaymentID": 1,
    "PaymentType": "Credit Card",
    "Amount": 100.50,
    "DateTime": "2023-10-01T12:00:00",
    "Status": "Completed"
}
```

---

### Retrieve All Payments
**Request**:
```http
GET /api/Payment
```

**Response**:
```json
200 OK
[
    {
        "PaymentID": 1,
        "PaymentType": "Credit Card",
        "Amount": 100.50,
        "DateTime": "2023-10-01T12:00:00",
        "Status": "Completed"
    }
]
```

---

### Error Handling
- **404 Not Found**: Returned when a requested payment does not exist.
- **400 Bad Request**: Returned when invalid data is provided in the request body.

---

## Notes
- The `PaymentController` uses an in-memory list to store payment records for demonstration purposes. In a production environment, this would typically be replaced with a database.
