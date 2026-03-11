# Acceptance Tests for AuthController

## **Feature:** User Registration

**Scenario:** Successfully register a new customer

- **Given** the API is running
- **When** a POST request is sent to `/api/auth/register` with body:
  ```json
  { "username": "Alice", "email": "alice@example.com", "password": "Passw0rd!", "role": "Customer" }
  ```
- **Then** the response should return status `201 Created`
- **And** the response body should include `id`, `username`, `email`, and `role`
- **And** the response body must NOT include `passwordHash`

**Scenario:** Fail to register with a duplicate email

- **Given** a user with `email = "alice@example.com"` already exists
- **When** a POST request is sent to `/api/auth/register` with the same email
- **Then** the response should return status `409 Conflict`
- **And** the response body should contain an error message indicating the email is already in use

**Scenario:** Fail to register with missing required fields

- **Given** the API is running
- **When** a POST request is sent to `/api/auth/register` with a missing `email` or `password`
- **Then** the response should return status `400 Bad Request`
- **And** the response body should describe which fields are invalid

**Scenario:** Fail to register with a password that is too short

- **Given** the API is running
- **When** a POST request is sent to `/api/auth/register` with a password shorter than 8 characters
- **Then** the response should return status `400 Bad Request`
- **And** the response body should indicate the password does not meet the minimum length requirement

---

## **Feature:** User Login

**Scenario:** Successfully log in with valid credentials

- **Given** a user exists with `email = "alice@example.com"` and `password = "Passw0rd!"`
- **When** a POST request is sent to `/api/auth/login` with body:
  ```json
  { "email": "alice@example.com", "password": "Passw0rd!" }
  ```
- **Then** the response should return status `200 OK`
- **And** the response body should include a `token` (non-empty string) and `expiresAt`

**Scenario:** Fail to log in with an incorrect password

- **Given** a user exists with `email = "alice@example.com"`
- **When** a POST request is sent to `/api/auth/login` with the wrong password
- **Then** the response should return status `401 Unauthorized`

**Scenario:** Fail to log in with an unknown email

- **Given** no user exists with `email = "unknown@example.com"`
- **When** a POST request is sent to `/api/auth/login` with that email
- **Then** the response should return status `401 Unauthorized`

**Scenario:** Fail to log in with missing fields

- **Given** the API is running
- **When** a POST request is sent to `/api/auth/login` with a missing `email` or `password`
- **Then** the response should return status `400 Bad Request`

---

## **Feature:** Accessing Protected Endpoints

**Scenario:** Successfully access a protected endpoint with a valid token

- **Given** the user has logged in and received a JWT token
- **When** a GET request is sent to `/api/orders` with header `Authorization: Bearer <token>`
- **Then** the response should return status `200 OK`

**Scenario:** Fail to access a protected endpoint without a token

- **Given** the API is running
- **When** a GET request is sent to `/api/orders` without an `Authorization` header
- **Then** the response should return status `401 Unauthorized`

**Scenario:** Fail to access a protected endpoint with an invalid token

- **Given** the API is running
- **When** a GET request is sent to `/api/orders` with header `Authorization: Bearer invalid.token.value`
- **Then** the response should return status `401 Unauthorized`

**Scenario:** Fail to access an admin-only endpoint as a Customer

- **Given** the user is authenticated with role `Customer`
- **When** a DELETE request is sent to `/api/products/1` with a valid Customer token
- **Then** the response should return status `403 Forbidden`

**Scenario:** Successfully access an admin-only endpoint as an Admin

- **Given** the user is authenticated with role `Admin`
- **When** a DELETE request is sent to `/api/products/1` with a valid Admin token
- **Then** the response should return status `204 No Content`

**Scenario:** Successfully access public endpoint without a token

- **Given** the API is running
- **When** a GET request is sent to `/api/products` without an `Authorization` header
- **Then** the response should return status `200 OK`
- **And** the response should contain a list of products
